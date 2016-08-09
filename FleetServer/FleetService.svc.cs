using FleetTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FleetServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FleetService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FleetService.svc or FleetService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class FleetService : IFleetService
    {
        // Registration
        public FleetClientToken RegisterClient()
        {
            return null;
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
