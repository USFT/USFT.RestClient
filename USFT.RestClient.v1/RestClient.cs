using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace USFT.RestClient.v1
{
    /// <summary>
    /// Communicates with USFT servers through synchronous HTTP calls
    /// </summary>
    public class UsftClient
    {
        /// <summary>
        /// Proxy to use for all HttpWebRequests
        /// </summary>
        public WebProxy Proxy = null;
        public AuthenticationMode Authentication = AuthenticationMode.USFT;

        /// <summary>
        /// Authentication mode to use for requests. USFT uses a secure API key and a hash, Basic uses username:password.
        /// </summary>
        public enum AuthenticationMode { 
            /// <summary>
            /// Authenticate using a secure API key and a per-request hash.
            /// </summary>
            USFT, 
            /// <summary>
            /// Authenticate using username and password. 
            /// IMPORTANT: do not attempt http:// calls with this mode, use only https://
            /// </summary>
            Basic };

        #region API addressing and information
        private const string auth_Prefix_USFT = "USFT ";
        private const string hashDataPrefix_default = "USFTRESTv1";

        // this standard was adopted from microsoft live, as detailed inhttp://msdn.microsoft.com/en-us/library/live/hh243648.aspx#http_verbs
        private const string verb_Create = "POST";
        private const string verb_Read = "GET";
        private const string verb_Update = "PUT";
        private const string verb_Delete = "DELETE";

        private const string uriBase_default = "https://api.usft.com/v1";

        private const string uriPath_Test = "/Test";

        private const string uriPath_ReadDevice = "/Device/{0}";
        private const string uriPath_UpdateDevice = "/Device";
        private const string uriPath_ReadAllDevices = "/Device";
        private const string uriPath_CreateDevice = "/Device";

        private const string uriPath_ReadLocationData = "/Location/{0}";
        private const string uriPath_ReadAllLocationData = "/Location";
        private const string uriPath_UpdateLocationData = "/Location";
        private const string uriPath_DeleteLocationData = "/Location/{0}";

        private const string uriPath_ReadAddress = "/Address/{0}";
        private const string uriPath_ReadAllAddresses = "/Address";
        private const string uriPath_ReadAddressesByGroup = "/Address?f=bygroup&fid={0}";
        private const string uriPath_UpdateAddress = "/Address/{0}";
        private const string uriPath_CreateAddress = "/Address";
        private const string uriPath_DeleteAddress = "/Address/{0}";

        private const string uriPath_ReadAddressGroup = "/AddressGroup/{0}";
        private const string uriPath_ReadAllAddressGroups = "/AddressGroup";
        private const string uriPath_UpdateAddressGroup = "/AddressGroup/{0}";
        private const string uriPath_CreateAddressGroup = "/AddressGroup";
        private const string uriPath_DeleteAddressGroup = "/AddressGroup/{0}";

        private const string uriPath_CreateAddressFence = "/AddressFence";
        private const string uriPath_DeleteAddressFence = "/AddressFence/{0}";
        private const string uriPath_ReadAddressFence = "/AddressFence/{0}";
        private const string uriPath_ReadAddressFences = "/AddressFence";
        private const string uriPath_ReadAddressFencesParams = "/AddressFence?addressid={0}&alertid={1}";
        private const string uriPath_UpdateAddressFence = "/AddressFence";

        private const string uriPath_CreateAlert = "/Alert";
        private const string uriPath_DeleteAlert = "/Alert/{0}";
        private const string uriPath_ReadAlert = "/Alert/{0}";
        private const string uriPath_ReadAlertByType = "/Alert?alerttype={0}";
        private const string uriPath_ReadAlerts = "/Alert";
        private const string uriPath_UpdateAlert = "/Alert";

        private const string uriPath_ImmediateReport = "/ImmediateReport/{0}";
        private const string uriPath_ImmediateReportFromTo = "/ImmediateReport/historyfromto?from={0}&to={1}&interval={2}";

        private const string uriPath_CreateDispatch = "/Dispatch";
        private const string uriPath_ReadDispatch = "/Dispatch/{0}";
        private const string uriPath_ReadDispatches = "/Dispatch";

        private const string uriPath_CreateReportRequest = "/ReportRequest";
        private const string uriPath_ReadReportRequest = "/ReportRequest/{0}";
        private const string uriPath_ReadReportRequests = "/ReportRequest";
        private const string uriPath_DownloadReportRequest = "/ReportRequest/{0}?download=xlsx";

        private const string uriPath_SendDeviceMessage = "/DeviceMessage";
        private const string uriPath_ReadDeviceMessage = "/DeviceMessage/{0}";
        private const string uriPath_ReadDeviceMessages = "/DeviceMessage";
        private const string uriPath_ReadDeviceMessagesFiltered = "/DeviceMessage?type={0}&status={1}";

        private const string uriPath_CreateServiceCall = "/ServiceCall";
        private const string uriPath_DeleteServiceCall = "/ServiceCall/{0}";
        private const string uriPath_ReadServiceCall = "/ServiceCall/{0}";
        private const string uriPath_ReadServiceCalls = "/ServiceCall";
        private const string uriPath_ReadServiceCallsByStatus = "/ServiceCall?status={0}";
        private const string uriPath_UpdateServiceCall = "/ServiceCall";

        private const string uriPath_ReadTime = "/Time";

        private const string uriPath_ReadAccounts = "/Account";
        private const string uriPath_ReadAccount = "/Account/{0}";
        private const string uriPath_ReadLoginToken = "/LoginToken/{0}";

        private const string uriAppend_Account = "?account={0}";
        private const string uriAppend_AccountAdditional = "&account={0}";

        #endregion

        #region Protected variables
        protected string uriBase = null;
        protected string userName = null;
        protected string privateKey = null;
        /// <summary>
        /// Prefix for building the hash
        /// </summary>
        protected string HashDataPrefix = hashDataPrefix_default;
        #endregion

        #region Initializers

        /// <summary>
        /// Create a new instance of the USFT Rest Client.
        /// </summary>
        /// <param name="UserName">User Name for your Account</param>
        /// <param name="privateKey">Private Token for your Account</param>
        public UsftClient(string UserName, string PrivateKeyOrPassword, AuthenticationMode AuthMode = AuthenticationMode.USFT)
        {
            this.userName = UserName;
            this.privateKey = PrivateKeyOrPassword;
            this.Authentication = AuthMode;
            SetUriBase(uriBase_default);
        }

        /// <summary>
        /// Creates a new instance of the USFT Rest Client.
        /// </summary>
        /// <param name="UserName">User Name for your Account</param>
        /// <param name="privateKey">Private Token for your Account</param>
        /// <param name="UriBase">Base URI for the service</param>
        public UsftClient(string UserName, string PrivateKey, string UriBase, AuthenticationMode AuthMode = AuthenticationMode.USFT)
        {
            userName = UserName;
            privateKey = PrivateKey;
            this.Authentication = AuthMode;
            SetUriBase(UriBase);
        }

        public void SetUriBase(string NewUriBase)
        {
            uriBase = NewUriBase;
            while (uriBase.EndsWith("/"))
                uriBase = uriBase.Substring(0, uriBase.Length - 1);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Tests the connection to USFT
        /// </summary>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the /Test method completes successfully</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public bool TestConnection(Account AsAccount = null)
        {
            HttpWebResponse response = AttemptRequest(uriPath_Test + AppendAccount(AsAccount), verb_Read);
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else throw RestException.Create(response);
        }

        /// <summary>
        /// Retrieves the current time according to the server. This can be used to coordinate system clocks for more precise Header information in later requests.
        /// </summary>
        /// <returns></returns>
        public DateTime ServerTime()
        {
            return RetrieveResponse<DateTime>(uriPath_ReadTime, verb_Read, null, false);
        }

        protected static class Security
        {
            static string GetString(byte[] bytes)
            {
                var hex = BitConverter.ToString(bytes);
                return hex.Replace("-", "").ToLower();
            }

            static byte[] GetBytes(string str)
            {
                return Encoding.ASCII.GetBytes(str);
            }

            public static string GenerateHash(string apiKey, string data)
            {
                using (var hmac = new HMACSHA512(GetBytes(apiKey)))
                {
                    return GetString(hmac.ComputeHash(GetBytes(data)));
                }
            }
        }
        #endregion

        #region Devices
        /// <summary>
        /// Gets information for a single device from USFT
        /// </summary>
        /// <param name="Id">ID of the device</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The requested Device</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public Device GetDevice(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<Device>(string.Format(uriPath_ReadDevice, Id) + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Gets information for all devices viewable by your account
        /// </summary>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The list of all devices viewable by your account</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public List<Device> GetDevices(Account AsAccount = null)
        {
            return RetrieveResponse<List<Device>>(uriPath_ReadAllDevices + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Update a device's entry with new information.
        /// </summary>
        /// <param name="DeviceToUpdate">A device entry with modified name, flag color, or text color. All other changes will be ignored.</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The device's entry, as of the most recent update.</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public Device UpdateDevice(Device DeviceToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<Device>(string.Format(uriPath_UpdateDevice, DeviceToUpdate.DeviceId) + AppendAccount(AsAccount), verb_Update, DeviceToUpdate);
        }

        /// <summary>
        /// Create a new virtual device entry. NOTE: restricted to contracted customers
        /// </summary>
        /// <param name="DeviceToCreate">The device entry to create</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The device's entry, as of the most recent update.</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public Device CreateDevice(Device DeviceToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<Device>(uriPath_CreateDevice + AppendAccount(AsAccount), verb_Create, DeviceToCreate);
        }

        #endregion

        #region DeviceLocations
        /// <summary>
        /// Get the latest location update from all of your account's gps devices
        /// </summary>
        /// <returns>A list of device ids and their most recent gps information</returns>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public List<DeviceLocation> GetDeviceLocations(Account AsAccount = null)
        {
            return RetrieveResponse<List<DeviceLocation>>(uriPath_ReadAllLocationData + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Get the latest location update for a given Device Id
        /// </summary>
        /// <param name="Id">The Id of the Device to query for location data</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The latest gps information for the specified Device</returns>
        /// <exception cref="RestException">Contains details of an unsuccessful request</exception>
        public DeviceLocation GetDeviceLocation(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<DeviceLocation>(string.Format(uriPath_ReadLocationData, Id) + AppendAccount(AsAccount), verb_Read);
        }

        public bool UpdateDeviceLocation(DeviceLocation UpdatedLocation)
        {
            return RetrieveResponse<bool>(uriPath_UpdateLocationData, verb_Update, UpdatedLocation);
        }

        public bool DeleteDeviceLocation(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteLocationData, Id), verb_Delete);
        }
        #endregion

        #region Addresses
        /// <summary>
        /// Retrieves all addresses viewable by your account
        /// </summary>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>A List of Address entries viewable by your account</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public List<Address> GetAddresses(Account AsAccount = null)
        {
            return RetrieveResponse<List<Address>>(uriPath_ReadAllAddresses + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Retrives all addresses associated with a certain Address Group
        /// </summary>
        /// <param name="Id">Id of the Address Group to search for Addresses</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>A List of Address entries associated with the Address Group</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public List<Address> GetAddressesByGroup(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<List<Address>>(string.Format(uriPath_ReadAddressesByGroup, Id) + AppendAccount(AsAccount, true), verb_Read);
        }

        /// <summary>
        /// Retrives all addresses associated with a certain Address Group
        /// </summary>
        /// <param name="Id">The Address Group to search for Addresses</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>A List of Address entries associated with the Address Group</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public List<Address> GetAddressesByGroup(AddressGroup Group, Account AsAccount = null)
        {
            return RetrieveResponse<List<Address>>(string.Format(uriPath_ReadAddressesByGroup, Group.AddressGroupId) + AppendAccount(AsAccount, true), verb_Read);
        }

        /// <summary>
        /// Retrieves information about a single Address
        /// </summary>
        /// <param name="Id">The Id of the Address to retrieve</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The Address requested</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public Address GetAddress(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<Address>(string.Format(uriPath_ReadAddress, Id) + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Creates a new Address entry for your account
        /// </summary>
        /// <param name="ToCreate">The Address object to create. AddressId should be set to 0, which is its default</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The newly created Address object as it exists in the database</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public Address CreateAddress(Address ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<Address>(uriPath_CreateAddress + AppendAccount(AsAccount), verb_Create, ToCreate);
        }

        /// <summary>
        /// Updates an existing Address with new information
        /// </summary>
        /// <param name="ToUpdate">The Address object to update, with its new modifications</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The Address object as it exists in the database, post-update</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public Address UpdateAddress(Address ToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<Address>(string.Format(uriPath_UpdateAddress, ToUpdate.AddressId) + AppendAccount(AsAccount), verb_Update, ToUpdate);
        }

        /// <summary>
        /// Deletes an existing Address from your account
        /// </summary>
        /// <param name="ToDelete">The Id of the Address to delete from your account</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeded, False if it failed</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public bool DeleteAddress(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteAddress, Id) + AppendAccount(AsAccount), verb_Delete);
        }

        /// <summary>
        /// Deletes an existing Address from your account
        /// </summary>
        /// <param name="ToDelete">The Address to delete from your account</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeded, False if it failed</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public bool DeleteAddress(Address ToDelete, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteAddress, ToDelete.AddressId) + AppendAccount(AsAccount), verb_Delete);
        }
        #endregion

        #region Address Groups
        /// <summary>
        /// Retrieves the list of Address Groups from your account
        /// </summary>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The list of Address Groups viewable by your account</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public List<AddressGroup> GetAddressGroups( Account AsAccount = null)
        {
            return RetrieveResponse<List<AddressGroup>>(uriPath_ReadAllAddressGroups + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Retrieves a specific Address Group from your account
        /// </summary>
        /// <param name="Id">Id of the Address Group to retrieve</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The requested Address Group</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressGroup GetAddressGroup(ulong Id, Account AsAccount = null)
        {
            return RetrieveResponse<AddressGroup>(string.Format(uriPath_ReadAddressGroup, Id) + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Updates an existing Address Group in your account
        /// </summary>
        /// <param name="ToUpdate">The Address Group to update, with its new modifications</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The updated Address Group, as it exists in the database</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressGroup UpdateAddressGroup(AddressGroup ToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<AddressGroup>(string.Format(uriPath_UpdateAddressGroup, ToUpdate.AddressGroupId) + AppendAccount(AsAccount), verb_Update, ToUpdate);
        }

        /// <summary>
        /// Creates a new Address Group in your account
        /// </summary>
        /// <param name="ToCreate">The Address Group to create in your account. AddressGroupId should be 0, which is its default</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The new Address Group object as it exists in the database</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressGroup CreateAddressGroup(AddressGroup ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<AddressGroup>(uriPath_CreateAddressGroup + AppendAccount(AsAccount), verb_Create, ToCreate);
        }

        /// <summary>
        /// Removes an existing Address Group from your account
        /// </summary>
        /// <param name="ToDelete">The Address Group to remove</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeded, false if it failed</returns>
        public bool DeleteAddressGroup(AddressGroup ToDelete, Account AsAccount = null)
        {
            return DeleteAddressGroup(ToDelete.AddressGroupId, AsAccount);
        }

        /// <summary>
        /// Removes an existing Address Group from your account
        /// </summary>
        /// <param name="AddressGroupId">The Id of the Address Group to remove</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeded, false if it failed</returns>
        public bool DeleteAddressGroup(ulong AddressGroupId, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteAddressGroup, AddressGroupId) + AppendAccount(AsAccount), verb_Delete);
        }
        #endregion

        #region Address Fences
        /// <summary>
        /// Retrieves Address Fences from your account
        /// </summary>
        /// <param name="AddressId">If specified, it filters for Fences associated with a specific Address ID</param>
        /// <param name="AlertId">If specified, it filters for the Fence associated with a specific Alert ID</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The Address Fences in your account that match any provided parameters</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public List<AddressFence> GetAddressFences(ulong AddressId = 0, ulong AlertId = 0, Account AsAccount = null)
        {
            //strictly speaking, we can always use the bottom request, but why not shave off a few bytes?
            if(AddressId==0 && AlertId==0)
                return RetrieveResponse<List<AddressFence>>(uriPath_ReadAddressFences + AppendAccount(AsAccount), verb_Read);
            else
                return RetrieveResponse<List<AddressFence>>(string.Format(uriPath_ReadAddressFencesParams, AddressId, AlertId) + AppendAccount(AsAccount, true), verb_Read);
        }

        /// <summary>
        /// Retrieves a specific Address Fence from your account
        /// </summary>
        /// <param name="AddressFenceId">ID of the Address Fence to retrieve</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The Address Fence for the given ID</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressFence GetAddressFence(ulong AddressFenceId, Account AsAccount = null)
        {
            return RetrieveResponse<AddressFence>(uriPath_ReadAddressFence + AppendAccount(AsAccount), verb_Read);
        }

        /// <summary>
        /// Updates an existing Address Fence in your account
        /// </summary>
        /// <param name="ToUpdate">The Address Fence to update, with changes in place</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The Address Fence after its update</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressFence UpdateAddressFence(AddressFence ToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<AddressFence>(uriPath_UpdateAddressFence + AppendAccount(AsAccount), verb_Update, ToUpdate);
        }

        /// <summary>
        /// Create a new Address Fence for your account
        /// </summary>
        /// <param name="ToCreate">The Address Fence to create. AddressId must be set to an existing address, AlertId must be set to 0.</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>The new Address Fence created in your account</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public AddressFence CreateAddressFence(AddressFence ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<AddressFence>(uriPath_CreateAddressFence + AppendAccount(AsAccount), verb_Create, ToCreate);
        }

        /// <summary>
        /// Delete an existing Address Fence in your account
        /// </summary>
        /// <param name="ToDelete">The Address Fence to delete</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeds, false otherwise</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public bool DeleteAddressFence(AddressFence ToDelete, Account AsAccount = null)
        {
            return DeleteAddressFence(ToDelete.AddressFenceId, AsAccount);
        }

        /// <summary>
        /// Delete an existing Address Fence in your account
        /// </summary>
        /// <param name="AddressFenceId">The ID of the Address Fence to delete</param>
        /// <param name="AsAccount">(Optional) If this parameter is included, the method will be run on behalf of the specified child Account.</param>
        /// <returns>True if the deletion succeeds, false otherwise</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public bool DeleteAddressFence(ulong AddressFenceId, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteAddressFence, AddressFenceId) + AppendAccount(AsAccount), verb_Delete);
        }
        #endregion

        #region Alerts

        public Alert GetAlert(ulong AlertId, Account AsAccount = null)
        {
            return RetrieveResponse<Alert>(string.Format(uriPath_ReadAlert, AlertId) + AppendAccount(AsAccount), verb_Read);
        }

        public List<Alert> GetAlerts(string alerttype = null, Account AsAccount = null)
        {
            if (string.IsNullOrEmpty(alerttype))
                return RetrieveResponse<List<Alert>>(uriPath_ReadAlerts + AppendAccount(AsAccount), verb_Read);
            else
                return RetrieveResponse<List<Alert>>(string.Format(uriPath_ReadAlertByType, WebUtility.UrlEncode(alerttype)) + AppendAccount(AsAccount, true), verb_Read);
        }

        public Alert CreateAlert(Alert ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<Alert>(uriPath_CreateAlert + AppendAccount(AsAccount), verb_Create, ToCreate);
        }

        public Alert UpdateAlert(Alert ToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<Alert>(uriPath_UpdateAlert + AppendAccount(AsAccount), verb_Update, ToUpdate);
        }

        public bool DeleteAlert(Alert ToDelete, Account AsAccount = null)
        {
            return DeleteAlert(ToDelete.AlertId, AsAccount);
        }

        public bool DeleteAlert(ulong AlertId, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteAlert, AlertId) + AppendAccount(AsAccount), verb_Delete);
        }
        #endregion

        #region Device Messages
        public DeviceMessage GetDeviceMessage(ulong DeviceMessageId, Account AsAccount = null)
        {
            return RetrieveResponse<DeviceMessage>(string.Format(uriPath_ReadDeviceMessage, DeviceMessageId) + AppendAccount(AsAccount), verb_Read);
        }
        public List<DeviceMessage> GetDeviceMessages(MessageTypes? MessageType = null, MessageStatuses? MessageStatus = null, Account AsAccount = null)
        {
            if(MessageType.HasValue || MessageStatus.HasValue)
                return RetrieveResponse<List<DeviceMessage>>(string.Format(uriPath_ReadDeviceMessagesFiltered, (int?)MessageType, (int?)MessageStatus) + AppendAccount(AsAccount, true), verb_Read);
            else
                return RetrieveResponse<List<DeviceMessage>>(uriPath_ReadDeviceMessages + AppendAccount(AsAccount), verb_Read);
        }
        public DeviceMessage SendDeviceMessage(DeviceMessage ToSend, Account AsAccount = null)
        {
            return RetrieveResponse<DeviceMessage>(uriPath_SendDeviceMessage + AppendAccount(AsAccount), verb_Create, ToSend);
        }
        #endregion

        #region Dispatch
        public Dispatch GetDispatch(ulong DispatchId, Account AsAccount = null)
        {
            return RetrieveResponse<Dispatch>(string.Format(uriPath_ReadDispatch, DispatchId) + AppendAccount(AsAccount), verb_Read);
        }
        public List<Dispatch> GetDispatches(Account AsAccount = null)
        {
            return RetrieveResponse<List<Dispatch>>(uriPath_ReadDispatches + AppendAccount(AsAccount), verb_Read);
        }
        public Dispatch CreateDispatch(Dispatch ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<Dispatch>(uriPath_CreateDispatch + AppendAccount(AsAccount), verb_Create, ToCreate);
        }
        #endregion

        #region Service Calls
        public ServiceCall GetServiceCall(ulong ServiceCallId, Account AsAccount = null)
        {
            return RetrieveResponse<ServiceCall>(string.Format(uriPath_ReadServiceCall, ServiceCallId) + AppendAccount(AsAccount), verb_Read);
        }
        public List<ServiceCall> GetServiceCalls(string status = null, Account AsAccount = null)
        {
            if(string.IsNullOrEmpty(status))
                return RetrieveResponse<List<ServiceCall>>(uriPath_ReadServiceCalls + AppendAccount(AsAccount), verb_Read);
            else
                return RetrieveResponse<List<ServiceCall>>(string.Format(uriPath_ReadServiceCallsByStatus, WebUtility.UrlEncode(status)) + AppendAccount(AsAccount, true), verb_Read);
        }
        public ServiceCall CreateServiceCall(ServiceCall ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<ServiceCall>(uriPath_CreateServiceCall + AppendAccount(AsAccount), verb_Create, ToCreate);
        }
        public ServiceCall UpdateServiceCall(ServiceCall ToUpdate, Account AsAccount = null)
        {
            return RetrieveResponse<ServiceCall>(uriPath_UpdateServiceCall + AppendAccount(AsAccount), verb_Update, ToUpdate);
        }
        public bool DeleteServiceCall(ulong ServiceCallId, Account AsAccount = null)
        {
            return RetrieveResponse<bool>(string.Format(uriPath_DeleteServiceCall, ServiceCallId) + AppendAccount(AsAccount), verb_Delete);
        }
        public bool DeleteServiceCall(ServiceCall ToDelete, Account AsAccount = null)
        {
            return DeleteServiceCall(ToDelete.ServiceCallId, AsAccount);
        }
        #endregion

        #region ImmediateReports



        public List<DeviceLocation> GetHistory24(List<ulong> DeviceIds = null, Account AsAccount = null)
        {
            if (DeviceIds == null || DeviceIds.Count() == 0)
                return RetrieveResponse<List<DeviceLocation>>(string.Format(uriPath_ImmediateReport, "history24") + AppendAccount(AsAccount), verb_Read);
            else
                return RetrieveResponse<List<DeviceLocation>>(string.Format(uriPath_ImmediateReport, "history24") + AppendAccount(AsAccount), "POST", DeviceIds);
        }

        public List<DeviceLocation> GetHistoryFromTo(List<ulong> DeviceIds = null, DateTime? From = null, DateTime? To = null, uint? Interval = null, Account AsAccount = null)
        {

            string f = "", t = "";
            if (From.HasValue)
                f = WebUtility.UrlEncode(From.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"));
            if (To.HasValue)
                t = WebUtility.UrlEncode(To.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"));
            if (DeviceIds == null || DeviceIds.Count() == 0)
                return RetrieveResponse<List<DeviceLocation>>(string.Format(uriPath_ImmediateReportFromTo, f, t, Interval) + AppendAccount(AsAccount, true), verb_Read);
            else
                return RetrieveResponse<List<DeviceLocation>>(string.Format(uriPath_ImmediateReportFromTo, f, t, Interval) + AppendAccount(AsAccount, true), "POST", DeviceIds);

        }
        #endregion

        #region ReportRequest
        public ReportRequest GetReportRequest(ulong ReportRequestId, Account AsAccount = null)
        {
            return RetrieveResponse<ReportRequest>(string.Format(uriPath_ReadReportRequest, ReportRequestId) + AppendAccount(AsAccount), verb_Read);
        }
        public List<ReportRequest> GetReportRequests(Account AsAccount = null)
        {
            return RetrieveResponse<List<ReportRequest>>(uriPath_ReadReportRequests + AppendAccount(AsAccount), verb_Read);
        }
        public ReportRequest CreateReportRequest(ReportRequest ToCreate, Account AsAccount = null)
        {
            return RetrieveResponse<ReportRequest>(uriPath_CreateReportRequest + AppendAccount(AsAccount), verb_Create, ToCreate);
        }
        public Stream DownloadReport(ulong ReportRequestId, Account AsAccount = null)
        {
            HttpWebResponse response = AttemptRequest(string.Format(uriPath_DownloadReportRequest, ReportRequestId) + AppendAccount(AsAccount, true), verb_Read);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.GetResponseStream();
            }
            else throw RestException.Create(response);
        }
        #endregion

        #region Account Info
        public Account GetAccount(ulong AccountId)
        {
            return RetrieveResponse<Account>(string.Format(uriPath_ReadAccount, AccountId), verb_Read);
        }

        public List<Account> GetAccounts()
        {
            return RetrieveResponse<List<Account>>(uriPath_ReadAccounts, verb_Read);
        }
        #endregion

        #region LoginToken
        public LoginToken GetLoginToken(ulong AccountId)
        {
            return RetrieveResponse<LoginToken>(string.Format(uriPath_ReadAccount, AccountId), verb_Read);
        }

        public LoginToken GetLoginToken(Account AsAccount)
        {
            return GetLoginToken(AsAccount.AccountId);
        }
        #endregion


        #region Protected and Private methods

        /// <summary>
        /// Creates the account query parameter
        /// </summary>
        /// <param name="OptionalAccount">The account to simulate for this method call</param>
        /// <returns>"&account=[AccountID]" for a valid parameter, null otherwise</returns>
        private string AppendAccount(Account OptionalAccount, bool isAdditionalParameter = false)
        {
            if (OptionalAccount != null && OptionalAccount.AccountId != 0)
                return string.Format(isAdditionalParameter ? uriAppend_AccountAdditional : uriAppend_Account, OptionalAccount.AccountId);
            return null;
        }
        #endregion

        #region Networking methods
        public HttpWebResponse AttemptRequest(string Path, string Method, string PostString, bool DoAuthentication, DateTime? dateTimeOverride = null)
        {
            string address = uriBase + Path;
            
            Uri uri = new Uri(address);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            if (Proxy != null)
                request.Proxy = Proxy;

            if (DoAuthentication)
            {
                string AuthString;
                if (Authentication == AuthenticationMode.USFT)
                {
                    // Building the http Date header is a little tricky.
                    // HttpWebRequest insists on formatting the string itself, because incorrectly formatted values are very common.
                    DateTime d;
                    if (dateTimeOverride.HasValue)
                        d = dateTimeOverride.Value;
                    else d = DateTime.Now;
                    request.Date = d;

                    // So, we let it do so then ask how it decided to format that.
                    // The REST service will always use the literal string sent to the server, rather than rely on one date format or another.
                    string datestring = request.Headers["Date"];

                    // Everything else is cake.
                    string hash = Security.GenerateHash(privateKey, HashDataPrefix + uri.AbsolutePath + datestring);
                    AuthString = string.Format("{0}{1}:{2}", auth_Prefix_USFT, userName, hash);
                }
                else if(Authentication == AuthenticationMode.Basic)
                {
                    AuthString = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, privateKey)));
                }
                else
                {
                    throw new Exception("Unknown AuthenticationMode");
                }
                request.Headers.Add(HttpRequestHeader.Authorization, AuthString);
            }

            request.Method = Method;
            request.ServicePoint.Expect100Continue = false;

            if (!string.IsNullOrEmpty(PostString))
            {

                byte[] postBytes = Encoding.ASCII.GetBytes(PostString);

                request.ContentType = "application/json";
                request.ContentLength = postBytes.Length;

                Stream postStream = request.GetRequestStream();
                postStream.Write(postBytes, 0, postBytes.Length);
                postStream.Flush();
            }

            

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch (WebException exc)
            {
                if (exc.Response != null)
                    throw RestException.Create((HttpWebResponse)exc.Response);
                else
                    throw new RestException("No response generated", exc);
            }
            catch (Exception exc)
            {
                throw new RestException("Unexpected exception encountered while attempting HttpWebRequest.GetResponse()", exc);
            }
        }

        /// <summary>
        /// Create and send the HttpWebRequest
        /// </summary>
        /// <param name="Path">Path string of the method to call</param>
        /// <param name="Method">HTTP Method string (Default "GET")</param>
        /// <param name="BodyContent">Object to send the request body</param>
        /// <returns>HttpWebResponse returned from the server</returns>
        public HttpWebResponse AttemptRequest(string Path, string Method = "GET", object BodyContent = null, bool DoAuthentication = true)
        {
            string PostString = null;
            if (BodyContent != null)
                PostString = JsonConvert.SerializeObject(BodyContent);
            return AttemptRequest(Path, Method, PostString, DoAuthentication);
        }

        /// <summary>
        /// Send a custom request to the API
        /// </summary>
        /// <typeparam name="T">Type of the object you expect in return</typeparam>
        /// <param name="Path">Path of the API call, eg: "/Test"</param>
        /// <param name="Method">HTTP Verb to use, eg: "GET", "POST", etc.</param>
        /// <param name="BodyContent">Object to submit in the request body, usually for Creating or Updating records</param>
        /// <returns>Object returned by the database</returns>
        /// <exception cref="RestException">A RestException containing the information returned by the server for an unsuccessful request</exception>
        public T RetrieveResponse<T>(string Path, string Method = "GET", object BodyContent = null, bool DoAuthentication = true)
        {
            HttpWebResponse response = AttemptRequest(Path, Method, BodyContent, DoAuthentication);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                T ret = JsonConvert.DeserializeObject<T>(content);
                return ret;
            }
            else throw RestException.Create(response);
        }
        #endregion

        
    }
}
