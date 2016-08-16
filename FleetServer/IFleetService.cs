using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FleetServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFleetService" in both code and config file together.
    [ServiceContract]
    public interface IFleetService
    {
        // Registration
        [OperationContract]
        FleetClientToken RegisterClient(FleetClientRegistration registrationModel);

        //  Heartbeat
        [OperationContract]
        FleetHearbeatEnum Heartbeat(FleetClientToken token, IEnumerable<FleetClientIdentifier> knownClients);

        // Files
        [OperationContract]
        List<FleetFileIdentifier> QueryFiles(FleetClientToken token);

        [OperationContract]
        FleetFile GetFile(FleetClientToken token, FleetFileIdentifier fileId);

        [OperationContract(Name = "SendFileSingleRecipient")]
        Boolean SendFile(FleetClientToken token, FleetClientIdentifier recipient, FleetFile file);

        [OperationContract(Name = "SendFileMultipleRecipient")]
        Boolean SendFile(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetFile file);

        // Messages

        [OperationContract]
        List<FleetMessageIdentifier> QueryMessages(FleetClientToken token);

        [OperationContract]
        FleetMessage GetMessage(FleetClientToken token, FleetMessageIdentifier fileId);

        [OperationContract(Name = "SendMessageSingleRecipient")]
        Boolean SendMessage(FleetClientToken token, FleetClientIdentifier recipient, FleetMessage msg);

        [OperationContract(Name = "SendMessageMultipleRecipient")]
        Boolean SendMessage(FleetClientToken token, List<FleetClientIdentifier> recipients, FleetMessage msg);
    }
}
