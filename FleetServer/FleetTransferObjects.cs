using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FleetTransferObjects
{

    [DataContract]
    public class FleetClientRegistration
    {
        [DataMember]
        public string FriendlyName { get; set; }

        [DataMember]
        public string IpAddress { get; set; }

        [DataMember]
        public string MacAddress { get; set; }
    }

    [DataContract]
    public class FleetFile
    {
        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public Byte[] FileContents { get; set; }
    }

    [DataContract]
    public class FleetFileIdentifier
    {
        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public String Identifier { get; set; }

        [DataMember]
        public Int32 FileSize { get; set; }
    }

    [DataContract]
    public class FleetClientToken
    {
        [DataMember]
        public String Identifier { get; set; }

        [DataMember]
        public String Token { get; set; }
    }

    [DataContract]
    public class FleetClientIdentifier
    {
        [DataMember]
        public String Identifier { get; set; }

        [DataMember]
        public String WorkstationName { get; set; }
    }

    [DataContract]
    public enum FleetHearbeatEnum
    {
        [EnumMember]
        ClientUpdate = 1 << 0,

        [EnumMember]
        ControlUpdate = 1 << 1,

        [EnumMember]
        ManageUpdate = 1 << 2,

        [EnumMember]
        FileAvailable = 1 << 3
    }
}