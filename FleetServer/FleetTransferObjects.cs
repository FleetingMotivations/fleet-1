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
        /// <summary>
        /// Unique string identifiying a client
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// The unique name representing the room that the client is in
        /// </summary>
        [DataMember]
        public string RoomIdentifier { get; set; }
    }

    [DataContract]
    public class FleetControlStatus
    {
        /// <summary>
        /// PKEY of the workgroup 
        /// </summary>
        [DataMember]
        public int WorkgroupId { get; set; }

        /// <summary>
        /// Indicates if a client has permission to share within the context of a workgroup
        /// </summary>
        [DataMember]
        public bool CanShare { get; set; }

        /// <summary>
        /// Collection of the allowed applications within this workgroup
        /// </summary>
        [DataMember]
        public List<FleetApplicationIdentifier> AllowedApplications { get; set; }
    }

    [DataContract]
    public class FleetFile
    {
        /// <summary>
        /// The name of the file as it appears on the OS
        /// </summary>
        [DataMember]
        public String FileName { get; set; }

        /// <summary>
        /// Raw Contents of the file
        /// </summary>
        [DataMember]
        public Byte[] FileContents { get; set; }
    }

    /// <summary>
    /// Identifies a file within the Fleet system
    /// </summary>
    [DataContract]
    public class FleetFileIdentifier
    {
        /// <summary>
        /// The filename of the file. These can be the same amoung different files being sent by
        /// different workstations
        /// </summary>
        [DataMember]
        public String FileName { get; set; }

        /// <summary>
        /// Identifier for the file
        /// </summary>
        [DataMember]
        public String Identifier { get; set; }

        /// <summary>
        /// Size of hte file on disk
        /// </summary>
        [DataMember]
        public Int32 FileSize { get; set; }

        /// <summary>
        /// The name of the sender of the file
        /// </summary>
        [DataMember]
        public string SenderName { get; set; }

        /// <summary>
        /// The unique identifier of the client that sent the file
        /// </summary>
        [DataMember]
        public String SenderIdentifier { get; set; }
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
        /// <summary>
        /// Unique idetneifer for the given client
        /// </summary>
        [DataMember]
        public String Identifier { get; set; }

        /// <summary>
        /// The friendly name of the given client
        /// </summary>
        [DataMember]
        public String WorkstationName { get; set; }

        /// <summary>
        /// The time at which this client last performed a heartbeat
        /// </summary>
        [DataMember]
        public DateTime? LastSeen { get; set; }

        /// <summary>
        /// The horizontal display value for this client within a room
        /// </summary>
        [DataMember]
        public float TopXRoomOffset { get; set; }

        /// <summary>
        /// The vertical display value for this client within a room
        /// </summary>
        [DataMember]
        public float TopYRoomOffset { get; set; }

        /// <summary>
        /// Indiciates if this client is a facilitator workstation
        /// </summary>
        [DataMember]
        public bool IsFacilitator { get; set; }

        /// <summary>
        /// The display colour for this client
        /// </summary>
        [DataMember]
        public string Colour { get; set; }
    }

    // Messages

    [DataContract]
    public class FleetMessageIdentifier
    {
        /// <summary>
        /// Unique identifier for this message
        /// </summary>
        [DataMember]
        public Int32 Identifier { get; set; }

        /// <summary>
        /// The application Id that this message targets
        /// </summary>
        [DataMember]
        public Int32 ApplicationId { get; set; }
    }

    [DataContract]
    public class FleetApplicationIdentifier
    {
        /// <summary>
        /// THe PKEy id of this application within the fleet system
        /// </summary>
        [DataMember]
        public int ApplicationId { get; set; }
        
        /// <summary>
        /// The friendly name for this application
        /// </summary>
        [DataMember]
        public string ApplicationName { get; set; }
    }

    [DataContract]
    public class FleetMessage
    {
        /// <summary>
        /// A unique identifier for this message
        /// </summary>
        [DataMember]
        public Int32 Identifier { get; set; }

        /// <summary>
        /// The sender of the message
        /// </summary>
        [DataMember]
        public String Sender { get; set; }

        /// <summary>
        /// PKEY of the target application for this message
        /// </summary>
        [DataMember]
        public Int32 ApplicationId { get; set; }

        [DataMember]
        public String Application { get; set; }

        /// <summary>
        /// The time at which this message was dispatched from the sender
        /// </summary>
        [DataMember]
        public DateTime Sent { get; set; }

        /// <summary>
        /// The content of the messages
        /// </summary>
        [DataMember]
        public String Message { get; set; }
    }

    [DataContract]
    public class FleetWorkstationHierachy
    {
        [DataMember]
        public List<FleetCampusIdentifier> Campuses { get; set; } 
    }

    [DataContract]
    public class FleetCampusIdentifier
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public List<FleetBuildingIdentifier> Buildings { get; set; } 
    }

    [DataContract]
    public class FleetBuildingIdentifier
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public List<FleetRoomIdentifier> Rooms { get; set; } 
    }

    [DataContract]
    public class FleetRoomIdentifier
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public List<FleetClientIdentifier> Clients { get; set; } 
    }

    /// <summary>
    /// Enum describing the possible updates that a client may experience
    /// </summary>
    [Flags]
    [DataContract]
    public enum FleetHearbeatEnum
    {
        [EnumMember]
        NoUpdates = 1 << 0, // 0 is generally reserved for the default value

        [EnumMember]
        InWorkgroup = 1 << 2,

        [EnumMember]
        ManageUpdate = 1 << 3,

        [EnumMember]
        FileAvailable = 1 << 3
    }

    /// <summary>
    /// Enum descrbing the contexts availabel to query workstations from
    /// </summary>
    [Flags]
    [DataContract]
    public enum FleetClientContext
    {
        [EnumMember]
        Room = 1 << 0,

        [EnumMember]
        Building = 1 << 2,

        [EnumMember]
        Campus = 1 << 3,

        [EnumMember]
        Workgroup = 1 << 4
    }
}