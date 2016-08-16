using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FleetEntityFramework.DAL;
using FleetEntityFramework.Models;
using FleetServer.Utils;
using System.IO;

namespace FleetServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FleetService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FleetService.svc or FleetService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class FleetService : IFleetService
    {

        // Registration
        public FleetClientToken RegisterClient(FleetClientRegistration registrationModel)
        {
            // For now the unique identifier will be the FriendlyName
            var hash = registrationModel.FriendlyName.GetHashCode().ToString();

            // Creates a new database context.  This is a bad way to do it, but
            // will suffice until we have DI
            using (var context = new FleetContext())
            {
                var workstation = context.Workstations
                        .FirstOrDefault(s => s.WorkstationIdentifier == hash);

                if (workstation == null)
                {
                    workstation = new FleetEntityFramework.Models.Workstation
                    {
                        FriendlyName = registrationModel.FriendlyName,
                        WorkstationIdentifier = hash,
                        IpAddress = registrationModel.IpAddress,
                        MacAddress = registrationModel.MacAddress,
                        LastSeen = DateTime.Now
                    };
                    context.Workstations.Add(workstation);

                    context.SaveChanges();
                }

                return new FleetClientToken
                {
                    //NOTE(Al): This is temporary, fix in future revisions
                    Identifier = workstation.WorkstationIdentifier,
                    Token = workstation.WorkstationIdentifier
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="knownClients">The querying clients list of current known clients</param>
        /// <returns></returns>
        public FleetHearbeatEnum Heartbeat(FleetClientToken token,
            IEnumerable<FleetClientIdentifier> knownClients)
        {
            FleetHearbeatEnum flags = FleetHearbeatEnum.NoUpdates;
            using (var context = new FleetContext())
            {
                var thisClient = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                thisClient.LastSeen = DateTime.Now;
                context.SaveChanges();

                var unseenMessages = context.MessageRecords
                    .Where(r => r.Target.WorkstationIdentifier == token.Identifier)
                    .Where(r => !r.HasBeenSeen);

                if (unseenMessages.Any())
                {
                    flags = flags.AddFlag(FleetHearbeatEnum.FileAvailable);
                    foreach (var message in unseenMessages)
                    {
                        message.HasBeenSeen = true;
                    }
                    context.SaveChanges();
                }

                var knownClientIdentifiers = knownClients.Select(c => c.WorkstationName);
                var newWorkstations = context.Workstations
                    .GetNewWorkstations(knownClientIdentifiers)
                    .Any(w => w.WorkstationIdentifier != token.Identifier);

                // TODO: Add concept of online for clients

                if (newWorkstations) flags = flags.AddFlag(FleetHearbeatEnum.ClientUpdate);
            }

            // If thre are updates, remove the no update flag from the enum
            if (flags.GetValues<FleetHearbeatEnum>().Any(flag => flags.HasFlag(flag)))
            {
                flags = flags.RemoveFlag(FleetHearbeatEnum.NoUpdates);
            }

            return flags;
        }

        /// <summary>
        /// Returns a manifest of unseen files for this client
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<FleetFileIdentifier> QueryFiles(FleetClientToken token)
        {
            using (var context = new FleetContext())
            {

                var records = context.MessageRecords
                    .Include(r => r.Message)
                    .Where(r => r.Target.WorkstationIdentifier == token.Identifier)
                    .Where(r => !r.HasBeenSeen)
                    .ToList() // Force query to resolve
                    .Where(r => r.Message.GetType() == (typeof(FileMessage)))
                    .Select(r => r.Message as FileMessage)
                    .Select(m => new FleetFileIdentifier
                    {
                        FileName = m.FileName,
                        FileSize = m.FileSize,
                        Identifier = m.MessageId.ToString()
                    }).ToList(); // Recast as list from IEnumerable

                return records;
            }
        }

        /// <summary>
        /// Returns a file given a fileId
        /// </summary>
        /// <param name="token">The client making the request</param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FleetFile GetFile(FleetClientToken token, FleetFileIdentifier fileId)
        {
            // TODO: Add security

            using (var context = new FleetContext())
            {
                var message = context.Messages
                    .OfType<FileMessage>()
                    .Single(m => m.MessageId == int.Parse(fileId.Identifier));

                var clientMessageRecord = context.MessageRecords
                    .Where(r => r.Message.MessageId == message.MessageId)
                    .Single(r => r.Target.WorkstationIdentifier == token.Identifier);

                clientMessageRecord.Received = DateTime.Now;

                var file = new FleetFile
                {
                    FileName = message.FileName,
                    FileContents = ReadFile(message.Uri)
                };

                context.SaveChanges();
                return file;
            }
        }

        public Boolean SendFile(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file)
        {
            using (var context = new FleetContext())
            {
                var sender = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                var receiver = context.Workstations.First(w => w.WorkstationIdentifier == recipient.Identifier);

                // TODO: resolve potential issues with write permissions against this directory
                var writePath = Directory.GetCurrentDirectory() + $"/temp/{sender.WorkstationIdentifier}/{file.FileName}_{DateTime.Now}";

                WriteFile(file.FileContents, writePath);

                var message = CreateFileManifest(sender, file, writePath, context);

                CreateFileRecords(message, new [] { receiver.WorkstationId }, context);
               
            }

            return true;
        }

        // Passing the context in will become better when we have dependancy injection
        /// <summary>
        /// Will create and save to the database a file manifest
        /// </summary>
        /// <param name="sender">The sending workstation of the file</param>
        /// <param name="file">The file information from the client</param>
        /// <param name="filePath">The filepath at which the file is saved</param>
        /// <param name="context">the DbContext (to be removed with DI)</param>
        /// <returns></returns>
        private Message CreateFileManifest(Workstation sender, FleetFile file, string filePath, FleetContext context)
        {
            var message = new FileMessage
                {
                    WorkstationId = sender.WorkstationId,
                    ApplicationId = context.Applications.First().ApplicationId,
                    // TODO: Update this to support application resolution
                    Sent = DateTime.Now,
                    FileName = file.FileName,
                    FileSize = file.FileContents.Length,    // Length in bytes D:
                    HasBeenScanned = false,
                    Uri = filePath
                };

            context.FileMessages.Add(message);
            context.SaveChanges();

            return message;
        }

        /// <summary>
        /// Will create a collecton of file records associated with a file and save to the database
        /// </summary>
        /// <param name="message">The message the records are associated with</param>
        /// <param name="targetIds">The id's of the clients to which the message is targeted at</param>
        /// <param name="context">Db context (to be removed with DI)</param>
        private void CreateFileRecords(Message message, int[] targetIds, FleetContext context)
        {
            IList<WorkstationMessage> records = targetIds.Select(t => new WorkstationMessage
            {
                WorkStationId = t,
                MessageId = message.MessageId,
                Received = null,
                HasBeenSeen = false
            }).ToList();

            message.MessageRecords = records;

            context.SaveChanges();
        }

        /// <summary>
        /// Writes a file to disk
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="path">The absolute path to which to write the file</param>
        private void WriteFile(byte[] contents, string path)
        {
            // TODO: Determine if there is a better way to do this
            try
            {
                using (var fileStream = new BinaryWriter(new FileStream(path, FileMode.CreateNew)))
                {
                    foreach (var chunk in contents)
                    {
                        fileStream.Write(chunk);
                    }
                }
            }
            catch (Exception e)
            {
                // Just give right up. 
                throw new Exception($"Unable to write file.\n{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Writes a file to disk
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="path">The absolute path to which to write the file</param>
        private byte[] ReadFile(string path)
        {
            // TODO: Determine if there is a better way to do this
            byte[] contents = null;

            try
            {
                contents = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                // Just give right up. 
                throw new Exception($"Unable to write file.\n{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
            }

            return contents;
        }

        public Boolean SendFile(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetFile file)
        {
            using (var context = new FleetContext())
            {
                var sender = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                var receivers = context.Workstations.Where(w => recipients.Select(r => r.Identifier).Contains(w.WorkstationIdentifier));
                var writePath = Directory.GetCurrentDirectory() + $"/temp/{sender.WorkstationIdentifier}/{file.FileName}_{DateTime.Now}";

                WriteFile(file.FileContents, writePath);

                var message = CreateFileManifest(sender, file, writePath, context);

                CreateFileRecords(message, receivers.Select(r => r.WorkstationId).ToArray(), context);
               
            }

            return true;
        }

        // Message

        public List<FleetMessageIdentifier> QueryMessages(FleetClientToken token)
        {
            return null;
        }

        public FleetMessage GetMessage(FleetClientToken token, FleetMessageIdentifier fileId)
        {
            return null;
        }

        public Boolean SendMessage(FleetClientToken token, FleetClientIdentifier recipient, FleetMessage msg)
        {
            return false;
        }

        public Boolean SendMessage(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetMessage msg)
        {
            return false;
        }
    }
}
