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
using System.Configuration;

namespace FleetServer
{
    /// <summary>
    /// Implements the base Fleet Service, 
    /// providing endpoints for querying clients and messages, 
    /// managing control changes and sending information between
    /// different workstatoins and applications running on them
    /// </summary>
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
                        RoomId = context.Rooms.Single(r => r.RoomIdentifier == registrationModel.RoomIdentifier).RoomId,
                        FriendlyName = registrationModel.FriendlyName,
                        WorkstationIdentifier = hash,
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
        ///  A polling endpoint for a Fleet client to determine message and control updates
        /// 
        /// If there are no updates, the return enum will indicate that no updates are available
        /// </summary>
        /// <param name="token">Token identifying the client</param>
        /// <returns>An enum indicating if this client has any unread messages or control status updates</returns>
        public FleetHearbeatEnum Heartbeat(FleetClientToken token) 
        {
            FleetHearbeatEnum flags = FleetHearbeatEnum.NoUpdates;
            using (var context = new FleetContext())
            {
                var thisClient = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                thisClient.LastSeen = DateTime.Now;
                context.SaveChanges();

                // Check for unseen messages
                var unseenMessages = context.MessageRecords
                    .Where(r => r.Target.WorkstationIdentifier == token.Identifier)
                    .Where(r => !r.HasBeenSeen);

                if (unseenMessages.Any())
                {
                    // We have unseen's, add the appropriate flags and mark as seen
                    flags = flags.AddFlag(FleetHearbeatEnum.FileAvailable);
                    flags = flags.AddFlag(FleetHearbeatEnum.ManageUpdate);
                    foreach (var message in unseenMessages)
                    {
                        message.HasBeenSeen = true;
                    }
                    context.SaveChanges();
                }

                // Check for any command / control updates
                var inProgress = Workgroup.IsInProgress().Compile();
                var inRunningWorkgroup = thisClient.Workgroups
                    .Where(wgr => !wgr.TimeRemoved.HasValue)
                    .Where(inProgress)
                    .Any();
                if (inRunningWorkgroup)
                {
                    flags = flags.AddFlag(FleetHearbeatEnum.InWorkgroup);
                }
            }

            // If thre are updates, remove the no update flag from the enum
            if (flags.GetValues<FleetHearbeatEnum>().Any(flag => flags.HasFlag(flag)))
            {
                flags = flags.RemoveFlag(FleetHearbeatEnum.NoUpdates);
            }

            return flags;
        }

        /// <summary>
        /// Returns the current control status of the client, or null if there are no control operations
        /// acting on the client
        /// </summary>
        /// <param name="token">Token identifying a fleet client</param>
        /// <returns>FleetControlStatus or null</returns>
        public FleetControlStatus QueryControlStatus(FleetClientToken token)
        {
            using (var context = new FleetContext())
            {
                var thisClient = context.Workstations.Single(w => w.WorkstationIdentifier == token.Identifier);

                // Query for any workgroups in which the client is a part of
                var inProgress = Workgroup.IsInProgress().Compile();
                var workgroup = thisClient.Workgroups
                    .Where(wgr => !wgr.TimeRemoved.HasValue)
                    .Where(inProgress)
                    .SingleOrDefault()?
                    .Workgroup;

                // No workgroup found
                if (workgroup == null)
                {
                    return null;
                }

                // Otherwise, return the required model
                // Handle the ability for a workstation to be kicked and 
                // readded to a workgroup
                return new FleetControlStatus
                {
                    WorkgroupId = workgroup.WorkgroupId,
                    CanShare = workgroup.Workstations
                        .Where(w => w.WorkstationId == thisClient.WorkstationId)
                        .OrderByDescending(wg => wg.TimeAdded)
                        .First()
                        .SharingEnabled,
                    AllowedApplications = workgroup.AllowedApplications
                        .Select(a => new FleetApplicationIdentifier
                        {
                            ApplicationId = a.ApplicationId,
                            ApplicationName = a.ApplicationName
                        })
                        .ToList()
                };
            }
        }

        /// <summary>
        /// Returns the current hierachy of workstations. That is, all workstations available
        /// Subset into their relevant contexts
        /// </summary>
        /// <returns></returns>
        public FleetWorkstationHierachy QueryWorkstationHierachy()
        {
            using (var context = new FleetContext())
            {
                // This really requires an auto-mapper, but the config and things needed for
                // that are outside the scope of an MVP
                var ret = new FleetWorkstationHierachy
                {
                    Campuses = context.Campuses.Select(c => new FleetCampusIdentifier
                    {
                        Name = c.CampusIdentifer,
                        Id = c.CampusId,
                        Buildings = c.Buildings.Select(b => new FleetBuildingIdentifier
                        {
                            Name = b.BuildingIdentifier,
                            Id = b.BuildingId,
                            Rooms = b.Rooms.Select(r => new FleetRoomIdentifier
                            {
                                Name = r.RoomIdentifier,
                                Id = r.RoomId,
                                Clients = r.Workstations.Select(w => new FleetClientIdentifier
                                {
                                    Identifier = w.WorkstationIdentifier,
                                    LastSeen = w.LastSeen ?? DateTime.MinValue,
                                    WorkstationName = w.FriendlyName,
                                    TopXRoomOffset = w.TopXRoomOffset,
                                    TopYRoomOffset = w.TopYRoomOffset,
                                    Colour = w.Colour,
                                    IsFacilitator = w.IsFacilitator
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };

                return ret;
            }
        }

        /// <summary>
        /// Will return a list of all the clients on the server, based on the given context and provided id
        /// 
        /// E.g. a workgroup context and an id will get all the workstations under that particular workgroup
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientContext">The context from which to retrieve clients</param>
        /// <param name="id">The id of the context from which to retrieve clients</param>
        /// <returns></returns>
        public List<FleetClientIdentifier> QueryClients(FleetClientToken token, FleetClientContext clientContext, int id)
        {
            using (var context = new FleetContext())
            {
                var thisWorkstation = context.Workstations
                    .Where(w => w.WorkstationIdentifier == token.Identifier);

                IEnumerable<Workstation> workstations;

                // Grab the appropriate collection of workstations
                switch (clientContext)
                {
                    case FleetClientContext.Room:
                    {
                        workstations = context.Rooms
                                .Single(r => r.RoomId == id).Workstations;
                        break;
                    }

                    case FleetClientContext.Building:
                    {
                        workstations = context.Buildings
                                .Single(b => b.BuildingId == id)
                                .Rooms.SelectMany(r => r.Workstations);
                        break;
                    }
                    
                    case FleetClientContext.Campus:
                    {
                        workstations = context.Campuses
                            .Single(c => c.CampusId == id)
                            .Buildings.SelectMany(b => b.Rooms)
                            .SelectMany(r => r.Workstations);
                        break;
                    }

                    case FleetClientContext.Workgroup:
                    {
                        workstations = context.WorkgroupMembers
                            .Where(wg => wg.WorkgroupId == id)
                            .Select(w => w.Workstation);
                        break;
                    }

                    default:
                    {
                        workstations = context.Rooms
                            .Single(r => r.RoomId == thisWorkstation.First().RoomId)
                            .Workstations;
                        break;
                    }
                }

                // Filter out the current workstation and transform
                workstations = workstations.Except(thisWorkstation);
                return workstations.Select(w => new FleetClientIdentifier
                {
                    WorkstationName = w.FriendlyName,
                    Identifier = w.WorkstationIdentifier,
                    LastSeen = w.LastSeen,
                    TopXRoomOffset = w.TopXRoomOffset,
                    TopYRoomOffset = w.TopYRoomOffset,
                    Colour = w.Colour,
                    IsFacilitator = w.IsFacilitator
                }).ToList();
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
                    .Where(r => r.Received == null)
                    .ToList() // Force query to resolve
                    .Where(r => r.Message is FileMessage)
                    .Select(r => r.Message as FileMessage)
                    .Select(m => new FleetFileIdentifier
                    {
                        FileName = m.FileName,
                        FileSize = m.FileSize,
                        Identifier = m.MessageId.ToString(),
                        SenderName = m.Sender.FriendlyName,
                        SenderIdentifier = m.Sender.WorkstationIdentifier

                    }).ToList(); // Recast as list from IEnumerable

                return records;
            }
        }

        /// <summary>
        /// Returns a file given a fileId
        /// </summary>
        /// <param name="token">The client making the request</param>
        /// <param name="fileId"></param>
        /// <returns>The requested file</returns>
        public FleetFile GetFile(FleetClientToken token, FleetFileIdentifier fileId)
        {
            using (var context = new FleetContext())
            {
                var fileIdParsed = int.Parse(fileId.Identifier);
                var message = context.Messages
                    .OfType<FileMessage>()
                    .Single(m => m.MessageId == fileIdParsed);

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

        /// <summary>
        /// Sends the given file to the recipient
        /// </summary>
        /// <param name="token">Token identifiying the sending client</param>
        /// <param name="recipient">Identifier uniquely identifying the target of the message</param>
        /// <param name="file">The file to be sent</param>
        /// <returns></returns>
        public Boolean SendFile(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file)
        {
            using (var context = new FleetContext())
            {
                var sender = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                var receiver = context.Workstations.First(w => w.WorkstationIdentifier == recipient.Identifier);

                // Build the write path 
                var writePath = $"{GenerateFilePath()}/{file.FileName}_{DateTime.Now.GetHashCode()}_{token.Identifier}";
                writePath = WriteFile(file.FileContents, writePath);

                var message = CreateFileManifest(sender, file, writePath, context);

                CreateFileRecords(message, new [] { receiver.WorkstationId }, context);
               
            }

            return true;
        }

        /// <summary>
        /// Generates the base file path that will be used to write files
        /// </summary>
        /// <returns></returns>
        private string GenerateFilePath()
        {
            //var path = $"temp/{workstationId}";
            var path = ConfigurationManager.AppSettings["FileStorage"];
            if (!Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
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
                    FileSize = file.FileContents.Length,    // Length in bytes
                    HasBeenScanned = false,
                    Uri = filePath,
                    FileType = "jpg" // TODO: Update this to support decent file types
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
        private string WriteFile(byte[] contents, string path)
        {
            try
            {
                // Make the path acceptble to windows
                path = path.Replace(':', '_');
                path = path.Replace(' ', '_');
                using (var fileStream = new BinaryWriter(new FileStream(path, FileMode.CreateNew)))
                {
                    foreach (var chunk in contents)
                    {
                        fileStream.Write(chunk);
                    }
                }
                return path;
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
                throw new Exception($"Unable to read file.\n{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}");
            }

            return contents;
        }

        public Boolean SendFile(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetFile file)
        {
            using (var context = new FleetContext())
            {
                var sender = context.Workstations.First(w => w.WorkstationIdentifier == token.Identifier);
                // TODO:
                var receiverIds = recipients.Select(r => r.Identifier); 
                var receivers = context.Workstations.Where(w => receiverIds.Contains(w.WorkstationIdentifier));
                var writePath = $"{GenerateFilePath()}/{file.FileName}_{DateTime.Now.GetHashCode()}_{token.Identifier}";

                // Slight code duplication around here to avoid rebuilding the database context for each
                // call that would be required to handle each workstation
                writePath = WriteFile(file.FileContents, writePath);

                var message = CreateFileManifest(sender, file, writePath, context);
                
                CreateFileRecords(message, receivers.Select(r => r.WorkstationId).ToArray(), context);
            }

            return true;
        }

        // Message handling
        /// <summary>
        /// Querys the inter-application messages that have been to the given clients
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<FleetMessageIdentifier> QueryMessages(FleetClientToken token)
        {
            using (var ctx = new FleetContext())
            {
                // Get unseen messages for client
                var messages = ctx.MessageRecords
                    .Include(r => r.Message)
                    .Where(mr => mr.Target.WorkstationIdentifier == token.Identifier)
                    .Where(r => r.Received == null)
                    .ToList()
                    .Where(mr => mr.Message is AppMessage)
                    .Select(mr => mr.Message as AppMessage)
                    .ToList();

                // Create message 
                var messageIdentifiers = new List<FleetMessageIdentifier>();
                foreach (var msg in messages)
                {
                    var msgId = new FleetMessageIdentifier();
                    msgId.ApplicationId = msg.ApplicationId;
                    msgId.Identifier = msg.MessageId;
                    messageIdentifiers.Add(msgId);
                }

                return messageIdentifiers;
            }
        }
        /// <summary>
        /// Gets the specified message for the given client
        /// </summary>
        /// <param name="token">Token identifying this client</param>
        /// <param name="messageId">The message identifier of the message to be retreived</param>
        /// <returns></returns>
        public FleetMessage GetMessage(FleetClientToken token, FleetMessageIdentifier messageId)
        {
            using (var ctx = new FleetContext())
            {
                var message = ctx.AppMessages
                    .FirstOrDefault(msg => msg.MessageId == messageId.Identifier);

                if (message == null)
                {
                    throw new Exception("Invalid Message ID");
                }

                var clientMessageRecord = ctx.MessageRecords
                    .Where(r => r.Message.MessageId == message.MessageId)
                    .Single(r => r.Target.WorkstationIdentifier == token.Identifier);

                clientMessageRecord.Received = DateTime.Now;
                clientMessageRecord.HasBeenSeen = true;
                
                var sendMessage = new FleetMessage();
                sendMessage.ApplicationId = message.ApplicationId;
                sendMessage.Application = message.TargetApplication.ApplicationName;
                sendMessage.Sender = message.Sender.FriendlyName;
                sendMessage.Identifier = message.MessageId;
                sendMessage.Sent = message.Sent;
                sendMessage.Message = message.Message;
                
                ctx.SaveChanges();
                return sendMessage;
            }
        }

        public Boolean SendMessage(FleetClientToken token, FleetClientIdentifier recipient, FleetMessage msg)
        {
            var recipients = new List<FleetClientIdentifier>();
            recipients.Add(recipient);
            return SendMessage(token, recipients, msg);
        }

        public Boolean SendMessage(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetMessage msg)
        {
            using (var ctx = new FleetContext())
            {
                // Get the workstation record
                // todo: This will need to be updated
                Workstation senderWorkstation = ctx.Workstations
                    .Where(wks => wks.WorkstationIdentifier == token.Identifier)
                    .FirstOrDefault();
                // Get the application record
                Application targetApplication = ctx.Applications
                    .Where(app => app.ApplicationId == msg.ApplicationId)
                    .FirstOrDefault();

                // Make message
                var message = new AppMessage();
                message.TargetApplication = targetApplication;
                message.Message = msg.Message;
                message.Sender = senderWorkstation;
                message.Sent = DateTime.Now;

                IList<WorkstationMessage> records = recipients.Select(t => new WorkstationMessage
                {
                    WorkStationId = ctx.Workstations.Where(wks => wks.WorkstationIdentifier == t.Identifier).First().WorkstationId,
                    Message = message,
                    Received = null,
                    HasBeenSeen = false
                }).ToList();

                message.MessageRecords = records;

                // Create database records for each recipient
                foreach (var recipient in recipients)
                {
                    // Get recipient workstation record
                    Workstation recipientWorkstation = ctx.Workstations
                        .Where(wks => wks.WorkstationIdentifier == recipient.Identifier)
                        .FirstOrDefault();

                    var wksMessage = new WorkstationMessage();
                    wksMessage.Message = message;
                    wksMessage.Target = recipientWorkstation;
                }

                ctx.AppMessages.Add(message);
                
                ctx.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
