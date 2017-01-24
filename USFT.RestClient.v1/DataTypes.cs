namespace USFT.RestClient.v1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The possible types of fence for automatically-refreshing Address Fences.
    /// </summary>
    public enum FenceTypes
    {
        /// <summary>
        /// Do not use this value. This is returned for some entries which do not have a proper Fence Type set.
        /// </summary>
        Unknown,

        /// <summary>
        /// The Address Fence will redraw itself as a circle of radius [Dims] around the Address when the Address moves
        /// </summary>
        Circle,

        /// <summary>
        /// The Address Fence will redraw itself as a square of dimensions [Dims]x[Dims] centered on the Address when the Address moves.
        /// </summary>
        Square,

        /// <summary>
        /// The Address Fence will redraw itself as a rectangle of dimensions [Dims] centered on the Address when the Address moves.
        /// </summary>
        Rectangle
    };

    /// <summary>
    /// The supported units for Address Fence dimensions
    /// </summary>
    public enum FenceUnitTypes
    {
        /// <summary>
        /// Do not use this value. This is returned for some entries which do not have a proper Fence Unit Type set.
        /// </summary>
        Unknown,

        /// <summary>
        /// The Address Fence dimensions will be measured in feet.
        /// </summary>
        Feet,

        /// <summary>
        /// The Address Fence dimensions will be measured in yards.
        /// </summary>
        Yards,

        /// <summary>
        /// The Address Fence dimensions will be measured in miles.
        /// </summary>
        Miles,

        /// <summary>
        /// The Address Fence dimensions will be measured in meters.
        /// </summary>
        Meters,

        /// <summary>
        /// The Address Fence dimensions will be measured in kilometers.
        /// </summary>
        Kilometers
    };

    /// <summary>
    /// The supported Message Types for Device Messages
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Do not use this value. This is returned for some entries which do not have a proper Message Type set.
        /// </summary>
        Unknown,

        /// <summary>
        /// Command: The device receiving this message should lock its doors.
        /// </summary>
        LockDoor,

        /// <summary>
        /// Command: The device receiving this message should unlock its doors.
        /// </summary>
        UnlockDoor,

        /// <summary>
        /// Command: The device receiving this message should honk its horn.
        /// </summary>
        HonkHorn,

        /// <summary>
        /// Command: The device receiving this message should disable its engine.
        /// </summary>
        DisableEngine,

        /// <summary>
        /// Command: The device receiving this message should enable its engine.
        /// </summary>
        EnableEngine,

        /// <summary>
        /// Command: The device receiving this message should start its engine.
        /// </summary>
        StartEngine,

        /// <summary>
        /// Command: The device receiving this message should immediately transmit a location update.
        /// </summary>
        ForceTransmit,

        /// <summary>
        /// Command: The device receiving this message should immediately resync with available satellites.
        /// </summary>
        Resync,

        /// <summary>
        /// Command: An paramaterized command that varies depending on device.
        /// </summary>
        AdHoc = 14,

        /// <summary>
        /// Message: The device receiving this message should display the provided text to the driver.
        /// </summary>
        Text = 100,

        /// <summary>
        /// Message: This is a message from the driver of the device to the user of the API or Webclient.
        /// </summary>
        FromDevice = 101
    };

    /// <summary>
    /// The Status of an existing Device Message. Generally, this should not be set by API users.
    /// </summary>
    public enum MessageStatuses
    {
        /// <summary>
        /// A message request initiated on the webapp has been received and is awaiting processing.
        /// </summary>
        WebappReceived = 0x00,

        /// <summary>
        /// A message request initiated on the webapp has been received and is in processing.
        /// </summary>
        WebappProcessing = 0x01,

        /// <summary>
        /// A message request initiated on the webapp has been processed and sent.
        /// </summary>
        WebappProcessed = 0x02,

        /// <summary>
        /// A message request initiated on the webapp has encountered an unknown error.
        /// </summary>
        WebappErrorUnknown = 0x03,

        /// <summary>
        /// A message request initiated on the webapp has encountered an error with its SIM Number.
        /// </summary>
        WebappErrorSIMNumber = 0x04,

        /// <summary>
        /// A message request initiated on the webapp has encountered an error retrieving its MSISDN.
        /// </summary>
        WebappErrorMSISDN = 0x05,

        /// <summary>
        /// A message request initiated on the webapp has encountered an error: the device command requested is not supported.
        /// </summary>
        WebappErrorCommand = 0x06,

        /// <summary>
        /// A message request initiated on the webapp has encountered an error with smtp configuration.
        /// </summary>
        WebappErrorNoEmailAddress = 0x07,

        /// <summary>
        /// A message request initiated on the webapp has encountered an error with the telematic APIs.
        /// </summary>
        WebappErrorDeviceSIMInformation = 0x08,

        /// <summary>
        /// The text message has been sent to the device.
        /// </summary>
        TextSent = 40,

        /// <summary>
        /// The text message has been read by the device.
        /// </summary>
        TextRead = 41,

        /// <summary>
        /// The text message has been deleted by the device.
        /// </summary>
        TextDeleted = 42,

        /// <summary>
        /// A message request initiated through the API has been received and is being processed.
        /// </summary>
        APIProcessing = 51,

        /// <summary>
        /// A message request initiated through the API has been processed and sent.
        /// </summary>
        APIProcessed = 52,

        /// <summary>
        /// A message request initiated through the API has encountered an unknkown error.
        /// </summary>
        APIErrorUnknown = 53,

        /// <summary>
        /// A message request initiated through the API has encountered an error with its SIM Number.
        /// </summary>
        APIErrorSimNumber = 54,

        /// <summary>
        /// A message request initiated through the API has encountered an error with its MSISDN.
        /// </summary>
        APIErrorMSISDN = 55,

        /// <summary>
        /// A message request initiated through the API has encountered an error: the device command requested is not supported.
        /// </summary>
        APIErrorCommand = 56,

        /// <summary>
        /// A message request initiated through the API has encountered an error with SMTP configuration.
        /// </summary>
        APIErrorNoEmail = 57,

        /// <summary>
        /// A message request initiated through the API has encountered an error with the telematic API.
        /// </summary>
        APIErrorNoDeviceSimInfo = 58,

        /// <summary>
        /// A message request initiated through the API for a X5-Nav has been created and is awaiting processing.
        /// </summary>
        APIX5Created = 101,

        /// <summary>
        /// A message request initiated through the API for a X5-Nav has been processed and sent.
        /// </summary>
        APIX5Sent = 102,

        /// <summary>
        /// A message request initiated through the API for a X5-Nav has been received and acknowledged.
        /// </summary>
        APIX5Ack = 103,

        /// <summary>
        /// A message request initiated through the API for a X5-Nav has been answered.
        /// </summary>
        APIX5Answered = 104,

        /// <summary>
        /// A message request initiated through the API for a X5-Nav has prompted a response.
        /// </summary>
        APIX5Responded = 105
    };

    /// <summary>
    /// The types of reports which may be requested through /RequestedReport
    /// </summary>
    public enum ReportTypes
    {
        /// <summary>
        /// A report which checks for rapid mph changes in consecutive 10-second intervals
        /// </summary>
        AggressiveDriving = 1,

        /// <summary>
        /// Reports miles travelled by state, by device
        /// </summary>
        DeviceMileageByState = 2,

        /// <summary>
        /// Reports miles travelled by device
        /// </summary>
        DeviceMileage = 3,
        DeviceOperating = 4,
        ExcessiveIdling = 5,
        MasterListing = 6,
        PTOSensor = 7,
        SpeedAlert = 8,
        Speeding = 9,
        Standard = 10,
        StartStop = 11,
        PolygonZoneActivity = 12,
        TripsDetail = 13,
        History24,
        HistoryFromTo
    };

    public enum ReportStatus
    {
        Received = 0,
        Processing = 1,
        Processed = 2,
        Error = 3
    };

    /// <summary>
    /// This class represents Messages sent to active Devices, usually commands for the vehicle or text for the driver.
    /// </summary>
    public class DeviceMessage
    {
        /// <summary>
        /// The AccountId associated with the Message
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// The DeviceId targetted by the Message, or the originator of a Message sent from the device to the API.
        /// </summary>
        public ulong DeviceId;

        /// <summary>
        /// A unique identifier for the DeviceMessage. Use this when making further API calls specific to this message.
        /// </summary>
        public ulong MessageId;

        /// <summary>
        /// The Type of the Message. This must be set to send a successful message.
        /// </summary>
        public MessageTypes MessageType;

        /// <summary>
        /// The Status of the Message. If this is set prior to sending, it will be ignored.
        /// </summary>
        public MessageStatuses MessageStatus;

        /// <summary>
        /// The Text of a Message to be read by the driver.
        /// </summary>
        public string MessageText;

        /// <summary>
        /// The time the Message was sent.
        /// </summary>
        public DateTime? TimeSent;

        /// <summary>
        /// A list of prepared responses to the message. If set, this allows users of the X5-Nav to respond with the push of a button.
        /// </summary>
        public List<string> CannedResponses = new List<string>();

        /// <summary>
        /// A response supplied by a X5-Nav.
        /// </summary>
        public string X5Response;
    }

    /// <summary>
    /// This class represents USFT Devices.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// The AccountId associated with the Message
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for the Device. Use this when making further API calls specific to this device.
        /// </summary>
        public ulong DeviceId;

        /// <summary>
        /// The name of the device. This will show up on the device's label on the webapp.
        /// </summary>
        public string DeviceName;

        /// <summary>
        /// The color of the label's backdrop on the webapp.
        /// </summary>
        public string FlagColor;

        /// <summary>
        /// The color of the label's text on the webapp.
        /// </summary>
        public string TextColor;

        /// <summary>
        /// This provides the date the device was Activated, if known.
        /// </summary>
        public DateTime? ActivationDate;

        /// <summary>
        /// This is the relative URL of the device's icon.
        /// </summary>
        public string IconPath;

        public Device()
        {
        }

        public Device(ulong AccountId, ulong DeviceId, string DeviceName, string FlagColor, string TextColor,
            DateTime? ActivationDate = null, string IconPath = null)
        {
            this.AccountId = AccountId;
            this.DeviceId = DeviceId;
            this.DeviceName = DeviceName;
            this.FlagColor = FlagColor;
            this.TextColor = TextColor;
            this.ActivationDate = ActivationDate;
            this.IconPath = IconPath;
        }
    }

    /// <summary>
    /// This class represents Address Fences, optional extensions of Addresses that automatically create and move Geofence Alerts centered on Addresses.
    /// </summary>
    public class AddressFence
    {
        /// <summary>
        /// A unique identifier for the Address Fence. Use this when making further API calls specific to this Address Fence.
        /// </summary>
        public ulong AddressFenceId;

        /// <summary>
        /// The AccountId associated with this Address Fence.
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// The Address for this Address Fence. This must be set when creating a new Address Fence.
        /// </summary>
        public ulong AddressId;

        /// <summary>
        /// The AlertId for the Alert created by this Address Fence. This should not be modified.
        /// </summary>
        public ulong AlertId;

        /// <summary>
        /// The shape the Address Fence will generate when created, or when the Address is moved.
        /// </summary>
        public FenceTypes? FenceType;

        /// <summary>
        /// The dimensions of the Address Fence, in the format "[Number]" for circles and squares, or "[Number]x[Number]" for rectangles.
        /// </summary>
        public string FenceDims;

        /// <summary>
        /// The unit to be used when interpreting FenceDims.
        /// </summary>
        public FenceUnitTypes? FenceUnits;

        public AddressFence()
        {
        }

        public AddressFence(ulong AccountId, ulong AddressFenceId, ulong AddressId, ulong AlertId, FenceTypes FenceType,
            string FenceDims, FenceUnitTypes FenceUnits)
        {
            this.AccountId = AccountId;
            this.AddressFenceId = AddressFenceId;
            this.AddressId = AddressId;
            this.AlertId = AlertId;
            this.FenceType = FenceType;
            this.FenceDims = FenceDims;
            this.FenceUnits = FenceUnits;
        }
    }

    /// <summary>
    /// This class represents an Address in your account.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The AccountId associated with this Address Fence.
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for the Address. Use this when making further API calls specific to this Address.
        /// </summary>
        public ulong AddressId;

        /// <summary>
        /// The name assigned to this address. It will be printed as a label on the Webapp when the Address is on screen.
        /// </summary>
        public string Name;

        /// <summary>
        /// The address itself, eg: "123 Example St., Oklahoma City, OK 73111"
        /// </summary>
        public string AddressBlock;

        /// <summary>
        /// The Latitude of the Address. If Latitude and Longitude are set to 0 for the creation of a new Address, they will be automatically generated from the AddressBlock.
        /// </summary>
        public double Latitude;

        /// <summary>
        /// The Latitude of the Address. If Latitude and Longitude are set to 0 for the creation of a new Address, they will be automatically generated from the AddressBlock.
        /// </summary>
        public double Longitude;

        /// <summary>
        /// The USFT Image to be used when displaying the address on screen.
        /// </summary>
        public string Image;

        /// <summary>
        /// A list of AddressGroupIds referencing Address Groups to which this Address belongs. This Address can be added to an Address Group by adding the appropriate AddressGroupId to this List, then Updating the Address.
        /// </summary>
        public List<ulong> AddressGroupIds = new List<ulong>();

        public Address()
        {
        }

        public Address(ulong AccountId, ulong AddressId, string Name, string AddressBlock, double Latitude,
            double Longitude, string Image,
            List<ulong> AddressGroupIds = null)
        {
            this.AccountId = AccountId;
            this.AddressId = AddressId;
            this.Name = Name;
            this.AddressBlock = AddressBlock;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Image = Image;
            if (AddressGroupIds == null)
                this.AddressGroupIds = new List<ulong>();
            else this.AddressGroupIds = AddressGroupIds;
        }
    }

    /// <summary>
    /// This class represents an Address Group in your account
    /// </summary>
    public class AddressGroup
    {
        /// <summary>
        /// The AccountId associated with this Address Fence.
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for the Address Group. Use this when making further API calls specific to this Address Group.
        /// </summary>
        public ulong AddressGroupId;

        /// <summary>
        /// The name assigned to this Address Group. This will show up in the Address Group editor in the webapp.
        /// </summary>
        public string Name;

        /// <summary>
        /// A list of the AddressIds which belong to the Addresses in this Address Group. Addresses can be added or removed from the group by adding or removing their Ids from this List, then Updating the Address Group.
        /// </summary>
        public List<ulong> AddressIds = new List<ulong>();

        public AddressGroup()
        {
        }

        public AddressGroup(ulong AccountId, ulong AddressGroupId, string Name, List<ulong> AddressIds = null)
        {
            this.AccountId = AccountId;
            this.AddressGroupId = AddressGroupId;
            this.Name = Name;
            if (AddressIds == null)
                this.AddressIds = new List<ulong>();
            else this.AddressIds = AddressIds;
        }
    }

    /// <summary>
    /// This class represents the location of a Device
    /// </summary>
    public class DeviceLocation
    {
        /// <summary>
        /// The AccountId associated with the Device
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// The DeviceId associated with the Device
        /// </summary>
        public ulong DeviceId;

        /// <summary>
        /// The name of the Device at the time of the location transmission
        /// </summary>
        public string DeviceName;

        /// <summary>
        /// The Latitude of the Device at the time of the location transmission
        /// </summary>
        public double Latitude;

        /// <summary>
        /// The Longitude of the Device at the time of the location transmission
        /// </summary>
        public double Longitude;

        /// <summary>
        /// The Heading of the Device at the time of the location transmission, in degrees
        /// </summary>
        public int Heading;

        /// <summary>
        /// The Velocity of the Device at the time of the location transmission, in MPH
        /// </summary>
        public int Velocity;

        /// <summary>
        /// The number of satellites available to the Device at the time of transmission. Most Devices will only transmit when there are at least four.
        /// </summary>
        public int Satellites;

        /// <summary>
        /// The Ignition state of the Device at the time of transmission
        /// </summary>
        public int Ignition;

        /// <summary>
        /// The last time the Device had detectably moved, at the time of the location transmission. NOTE: Presently this is returned as null for history immediatereports.
        /// </summary>
        public DateTime? LastMoved;

        /// <summary>
        /// The last time the Device had sent an update status, at the time of the location transmission. This will almost always be the time of the location transmission.
        /// </summary>
        public DateTime LastUpdated;

        /// <summary>
        /// This contains raw output information for devices with PTO/Ignition/Panic capabilities.
        /// </summary>
        public uint OutputFlags;

        /// <summary>
        /// This contains the power level as reported by the device, usually 0-100 for devices capable of reporting power.
        /// </summary>
        public int Power;

        public DeviceLocation()
        {
        }

        public DeviceLocation(ulong AccountId
            , ulong DeviceId
            , string DeviceName
            , double Latitude
            , double Longitude
            , int Heading
            , int Velocity
            , int Satellites
            , int Ignition
            , DateTime LastUpdated
            , DateTime? LastMoved = null
            , uint OutputFlags = 0
            , int Power = 0)
        {
            this.AccountId = AccountId;
            this.DeviceId = DeviceId;
            this.DeviceName = DeviceName;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Heading = Heading;
            this.Velocity = Velocity;
            this.Satellites = Satellites;
            this.Ignition = Ignition;
            this.LastMoved = LastMoved;
            this.LastUpdated = LastUpdated;
            this.OutputFlags = OutputFlags;
            this.Power = Power;
        }
    }

    /// <summary>
    /// This class represents a point on the map for classes which require large collections of points
    /// </summary>
    public class MapPoint
    {
        /// <summary>
        /// The Latitude value for the point
        /// </summary>
        public double Latitude;

        /// <summary>
        /// The Longitude value for the point
        /// </summary>
        public double Longitude;

        public MapPoint(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        public override string ToString()
        {
            return Latitude.ToString() + "," + Longitude.ToString();
        }
    }

    /// <summary>
    /// This class represents a Service Call created in your account.
    /// </summary>
    public class ServiceCall
    {
        /// <summary>
        /// The AccountId associated with this ServiceCall
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for the Service Call. Use this when making further API calls specific to this Service Call.
        /// </summary>
        public ulong ServiceCallId;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string WorkOrder;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Description;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public DateTime? DispatchDate;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public DateTime? RequestDate;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public DateTime? EstimatedStartDate;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string CustomerName;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Caller;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string EquipmentId;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Item;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string TerritoryDesc;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Technician;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string BillCode;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string CallType;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Priority;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Status;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Comment;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Street;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string City;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string State;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string ZipCode;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string EquipmentRemarks;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string Contact;

        /// <summary>
        /// An optional parameter that can be set for your Service Calls.
        /// </summary>
        public string ContactPhone;

        /// <summary>
        /// If Latitude and Longitude are left null, the API will attempt to retrieve their values from the Street, City, State, and ZipCoded values provided.
        /// </summary>
        public double? Latitude;

        /// <summary>
        /// If Latitude and Longitude are left null, the API will attempt to retrieve their values from the Street, City, State, and ZipCoded values provided.
        /// </summary>
        public double? Longitude;

        /// <summary>
        /// The color of the label this service call will display in the webapp, in 6-character RGB format (eg, "FF0000" for bright red)
        /// </summary>
        public string FlagColor;

        /// <summary>
        /// The color of the label text this service call will display in the webapp, in 6-character RGB format (eg, "0000FF" for bright green)
        /// </summary>
        public string TextColor;

        /// <summary>
        /// The content of the label text this service call will display in the webapp
        /// </summary>
        public string MarkerName;
    }

    /// <summary>
    /// This class represents a Dispatch message sent to a device in your account
    /// </summary>
    public class Dispatch
    {
        /// <summary>
        /// The AccountId associated with this Dispatch
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for this Dispatch. Use this when making API calls specific to this Dispatch.
        /// </summary>
        public ulong DispatchId;

        /// <summary>
        /// The Device targetted by this Dispatch.
        /// </summary>
        public ulong DeviceId;

        /// <summary>
        /// (Optional) The Latitude targetted by the Dispatch. If Latitude and Longitude are left null, the API will attempt to find their values using the provided Address information.
        /// </summary>
        public double? Latitude;

        /// <summary>
        /// (Optional) The Longitude targetted by the Dispatch. If Latitude and Longitude are left null, the API will attempt to find their values using the provided Address information.
        /// </summary>
        public double? Longitude;

        /// <summary>
        /// The Address targetted by the Dispatch, for all devices EXCEPT the X5-Navs
        /// </summary>
        public string AddressBlock;

        /// <summary>
        /// The Street targetted by the Dispatch, ONLY for X5-Navs
        /// </summary>
        public string X5Street;

        /// <summary>
        /// The City targetted by the Dispatch, ONLY for X5-Navs
        /// </summary>
        public string X5City;

        /// <summary>
        /// The State targetted by the Dispatch, ONLY for X5-Navs
        /// </summary>
        public string X5State;
    }

    /// <summary>
    /// This class represents a request by your account for a long-running report.
    /// </summary>
    public class ReportRequest
    {
        /// <summary>
        /// The AccountId associated with this rquest
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// A unique identifier for this ReportRequest. Use this when making API calls specific to this ReportRequest.
        /// </summary>
        public ulong ReportRequestId;

        /// <summary>
        /// The type of report to run. This is an enumeration of values 1-15.
        /// </summary>
        public ReportTypes ReportType;

        /// <summary>
        /// The email address to contact when the report is complete and ready to download. This may be set to values such as 'NONE' to avoid sending an email.
        /// </summary>
        public string Email;

        /// <summary>
        /// Optional parameter: The start of the timespan to report on, for reports which allow a timespan.
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// Optional parameter: The end of the timespan to report on, for reports which allow a timespan.
        /// </summary>
        public DateTime EndDate;

        /// <summary>
        /// Optional parameter: The TimeZoneId to check, for reports which allow a TimeZone to be set.
        /// </summary>
        public uint TimeZoneId;

        /// <summary>
        /// Optional parameter: A number of whole seconds.
        /// </summary>
        public int Seconds;

        /// <summary>
        /// Optional parameter: A speed in MPH.
        /// </summary>
        public int Speed;

        /// <summary>
        /// Optional parameter: A list of Devices to check.
        /// </summary>
        public List<ulong> DeviceIds;

        /// <summary>
        /// The time the report request was received by the server.
        /// </summary>
        public DateTime TimeReceived;

        /// <summary>
        /// The status of the report request. This is an enumeration of values from 0 to 4.
        /// </summary>
        public ReportStatus Status;

        /// <summary>
        /// The time the report request was processed by the server.
        /// </summary>
        public DateTime TimeProcessed;

        /// <summary>
        /// Optional parameter: A ZoneId.
        /// </summary>
        public ulong ZoneId;

        /// <summary>
        /// Optional parameter: An interval, in whole seconds.
        /// </summary>
        public uint Interval;
    }

    /// <summary>
    /// This represents an account within the USFT database. Access is read-only.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// This is the unique identifier for the Account. Use this when referencing the account for future Rest calls.
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// This is a list of child Accounts which are viewable by the Account. Most Rest calls can be run on behalf of child Accounts.
        /// </summary>
        public List<ulong> ViewableAccountIds;

        /// <summary>
        /// This is the Login used to access the Account on the Webapp.
        /// </summary>
        public string Login;

        /// <summary>
        /// This is the Name associated with the Account.
        /// </summary>
        public string Name;

        /// <summary>
        /// This is the Email address associated with the Account.
        /// </summary>
        public string Email;

        public Account()
        {
            ViewableAccountIds = new List<ulong>();
        }

        public Account(ulong AccountId, List<ulong> ViewableAccountIds, string Login, string Name, string Email)
        {
            this.AccountId = AccountId;
            this.ViewableAccountIds = ViewableAccountIds;
            this.Login = Login;
            this.Name = Name;
            this.Email = Email;
        }
    }

    /// <summary>
    /// This class contains a temporary login token which will allow users to use the webapp without logging in.
    /// </summary>
    public class LoginToken
    {
        /// <summary>
        /// The AccountId of the Account for this token.
        /// </summary>
        public ulong AccountId;

        /// <summary>
        /// The temporary token.
        /// </summary>
        public string Token;

        /// <summary>
        /// The server DateTime at which the Token will cease to function.
        /// </summary>
        public DateTime TokenExpiration;

        /// <summary>
        /// A link to the webapp which will automatically log into the requested Account. This link will function for at most two minutes.
        /// </summary>
        public string Link;
    }

    /// <summary>
    /// Details for an alert event log entry
    /// </summary>
    public class AlertLog
    {
        /// <summary>
        /// Does this alert-type logically includ entering or leaving
        /// </summary>
        public bool CanEnterExit;

        /// <summary>
        /// True: when exiting a zone.
        /// False: when entering a zone.
        /// NULL: when there is no zone to enter or leave.
        /// </summary>
        public bool? Exiting;

        /// <summary>
        /// Latitude of device triggering alert
        /// </summary>
        public double? Latitude;

        /// <summary>
        /// Longitude of device triggering alert
        /// </summary>
        public double? Longitude;

        /// <summary>
        /// The ID of the device triggering the alert
        /// </summary>
        public ulong DeviceId;

        /// <summary>
        /// The ID of the alert being triggered
        /// </summary>
        public ulong AlertId;

        /// <summary>
        /// Name of the alert being triggered
        /// </summary>
        public string AlertName;

        /// <summary>
        /// Description of the alert being triggered
        /// </summary>
        public string Description;

        /// <summary>
        /// Type of alert being triggered:
        /// Maintenance,Polyfence,Route,Panic,Ignition,Geofence,Speed,Power,Radius,Idle,Timeout
        /// </summary>
        public string AlertType;

        /// <summary>
        /// When the alert was triggered. UTC Datetime.
        /// </summary>
        public DateTime OcurredOn;
    }

    /// <summary>
        /// Represents an alert
        /// </summary>
        public class Alert
        {
            public enum AlertTypes
            {
                Geofence,
                Idle,
                Ignition,
                Maintenance,
                Panic,
                Polyfence,
                Power,
                Radius,
                Route,
                Speed,
                Timeout
            }

            public uint AlertId;
            public uint AccountId;
            public string AlertName;

            [JsonConverter(typeof (StringEnumConverter))] public AlertTypes AlertType;
            public string Color;
            public double? Distance;
            public double[][] Fence;
            public double? IdleThreshold;
            public bool? IgnitionOff;
            public bool? IgnitionOn;
            public double? MaintMeters;
            public double? MaintSeconds;
            public bool? PolyOnEnter;
            public bool? PolyOnExit;
            public int? PowerMinimum;
            public double? Radius;
            public bool? RadiusOnEnter;
            public bool? RadiusOnExit;
            public bool? RouteOnEnter;
            public bool? RouteOnExit;
            public string ShorthandEnd;
            public string ShorthandStart;
            public int? SpeedThreshold;
            public double? TimeoutThreshold;
            public double? WarnMeters;
            public double? WarnSeconds;
            public List<AlertMaintenance> MaintConditions = new List<AlertMaintenance>();
            public List<Recipient> Recipients = new List<Recipient>();
        }

        /// <summary>
        /// Represents a recipient associated with an alert
        /// </summary>
        public class Recipient
        {
            public bool SendEmail;
            public bool SendSms;
            public long RecipientId;
            public string Name;
            public string Email;
            public string PhoneNumber;
            public string SMS;
            public bool isDriver;
            public string Fob;
        }

        /// <summary>
        /// Represents maintenance details associated with an alert
        /// </summary>
        public class AlertMaintenance
        {
            public long? AlertMaintenanceId;
            public long AlertId;
            public string ConditionName;
            public bool OneTime;
            public double? AlertMeters;
            public double? AlertSeconds;
            public double? WarnMeters;
            public double? WarnSeconds;
        }
}