using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FleetServer
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

        [DataMember]
        public string SenderName { get; set; }
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

    // Messages

    [DataContract]
    public class FleetMessageIdentifier
    {
        [DataMember]
        public Int32 Identifier { get; set; }

        [DataMember]
        public Int32 ApplicationId { get; set; }
    }

    [DataContract]
    public class FleetMessage
    {
        [DataMember]
        public Int32 Identifier { get; set; }

        [DataMember]
        public Int32 ApplicationId { get; set; }

        [DataMember]
        public DateTime Sent { get; set; }

        [DataMember]
        public String Message { get; set; }
    }

    [Flags]
    [DataContract]
    public enum FleetHearbeatEnum
    {
        [EnumMember]
        NoUpdates = 1 << 0, // 0 is generally reserved for the default value

        [EnumMember]
        ClientUpdate = 1 << 1,

        [EnumMember]
        ControlUpdate = 1 << 2,

        [EnumMember]
        ManageUpdate = 1 << 3,

        [EnumMember]
        FileAvailable = 1 << 4
    }
}