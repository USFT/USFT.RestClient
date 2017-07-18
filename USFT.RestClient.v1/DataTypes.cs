namespace USFT.RestClient.v1
{
    using System;
    using System.Collections.Generic;

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
    /// This class represents vehicle information for a related device.
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// A unique identifier for the Device. Use this when making further API calls specific to this device.
        /// </summary>
        public ulong DeviceId;
        /// <summary>
        /// VIN of vehicle
        /// </summary>
        public string VIN;
        /// <summary>
        /// Fuel Card assigned to vehicle
        /// </summary>
        public string FuelCardNumber;
        /// <summary>
        /// Miles
        /// </summary>
        public ulong Miles;
        /// <summary>
        /// Hours
        /// </summary>
        public ulong Hours;

        public Vehicle() { }
        public Vehicle(ulong DeviceId, string VIN, string FuelCardNumber, ulong Miles, ulong Hours)
        {
            this.DeviceId = DeviceId;
            this.VIN = VIN;
            this.FuelCardNumber = FuelCardNumber;
            this.Miles = Miles;
            this.Hours = Hours;
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
        public AddressFence() { }
        public AddressFence(ulong AccountId, ulong AddressFenceId, ulong AddressId, ulong AlertId, FenceTypes FenceType, string FenceDims, FenceUnitTypes FenceUnits)
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
    /// This class represents the location of a Device
    /// </summary>
    public class Location
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

        public Location()
        {
        }

        public Location(ulong AccountId
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
}