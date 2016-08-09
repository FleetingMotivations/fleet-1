using FleetTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FleetEntityFramework.DAL;

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

        //  Heartbeat
        public FleetHearbeatEnum Heartbeat(FleetClientToken token)
        {
            return FleetHearbeatEnum.FileAvailable;
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
