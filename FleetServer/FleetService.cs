using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FleetEntityFramework.DAL;
using FleetEntityFramework.Models;
using FleetServer.Utils;

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
        /// Will return a list of all the clients on the server
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<FleetClientIdentifier> QueryClients(FleetClientToken token)
        {
            using (var context = new FleetContext())
            {
                var thisWorkstation = context.Workstations
                    .Where(w => w.WorkstationIdentifier == token.Identifier);

                var allWorkstations = context.Workstations
                    .Except(thisWorkstation)
                    .Select(w => new FleetClientIdentifier
                        {
                            Identifier = w.WorkstationIdentifier,
                            WorkstationName = w.FriendlyName
                        });

                return allWorkstations.ToList();
            }
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

                // TODO: Load file 

                var file = new FleetFile
                {
                    FileName = message.FileName,
                    FileContents = new byte[] { }
                };

                context.SaveChanges();
                return file;
            }
        }

        public Boolean SendFile(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file)
        {
            // TODO: this

            return false;
        }

        public Boolean SendFile(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetFile file)
        {
            return false;
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
