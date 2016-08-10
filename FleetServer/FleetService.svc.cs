using FleetTransferObjects;
using System;
using System.Collections.Generic;
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
            // For now the unique hash will be the FriendlyName + Now hashed
            var uniqueHash = (registrationModel.FriendlyName + DateTime.Now.ToString() 
                /*+ registrationModel.IpAddress + registrationModel.MacAddress*/)
                            .GetHashCode()
                            .ToString(); // Super hacks for now

            // TODO: Update to use dependency injection
            using (var context = new FleetContext())
            {
                var workstation = context.Workstations
                        .FirstOrDefault(s => s.WorkstationIdentifier == uniqueHash);
                if (workstation == null)
                {
                    context.Workstations.Add(new FleetEntityFramework.Models.Workstation
                    {
                        FriendlyName = registrationModel.FriendlyName,
                        WorkstationIdentifier = uniqueHash,
                        IpAddress = registrationModel.IpAddress,
                        MacAddress = registrationModel.MacAddress,
                        LastSeen = DateTime.Now
                    });

                    context.SaveChanges();
                }
            }
            return new FleetClientToken
            {
                Identifier = uniqueHash,
                Token = uniqueHash
            };
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
                var unseenMessages = context.MessageRecords
                    .Where(r => r.Target.WorkstationIdentifier == token.Token)
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

                if (newWorkstations) flags.AddFlag(FleetHearbeatEnum.ClientUpdate);
            }

            // If thre are updates, remove the no update flag from the enum
            if (flags.GetValues<FleetHearbeatEnum>().Any(flag => flags.HasFlag(flag)))
            {
                flags.RemoveFlag(FleetHearbeatEnum.NoUpdates);
            }

            return flags;
        }

        // Files
        public List<FleetFileIdentifier> QueryFiles(FleetClientToken token)
        {
            return null;
        }

        public FleetFile GetFile(FleetClientToken token, FleetFileIdentifier fileId)
        {
            return null;
        }

        public Boolean SendFile(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file)
        {
            return false;
        }

        public Boolean SendFile(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetFile file)
        {
            return false;
        }
    }
}
