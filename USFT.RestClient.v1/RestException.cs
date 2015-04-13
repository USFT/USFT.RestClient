using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace USFT.RestClient.v1
{
    /// <summary>
    /// The RestException class interprets unsuccessful HTTP requests and generates descriptive exception messages from them.
    /// </summary>
    public class RestException : Exception
    {
        public RestException() { }
        public RestException(string message) : base(message) { }
        public RestException(string message, Exception innerException) : base(message, innerException) { }

        public HttpWebResponse UnsuccessfulResponse;

        /// <summary>
        /// Create a new RestException from an unsuccessful HttpWebResponse.
        /// </summary>
        /// <param name="UnsuccessfulResponse">The HttpWebResponse that failed to return successfully.</param>
        /// <returns>A RestException with Message determined by the HttpWebResponse info.</returns>
        public static RestException Create(HttpWebResponse UnsuccessfulResponse)
        {
            StreamReader reader = new StreamReader(UnsuccessfulResponse.GetResponseStream());
            string content = reader.ReadToEnd();
            RestException ret = new RestException();
            
            try
            {
                UnsuccessfulBody body = JsonConvert.DeserializeObject<UnsuccessfulBody>(content);
                ret = new RestException(string.Format("{0} returned {1}: {2}", UnsuccessfulResponse.ResponseUri.PathAndQuery, UnsuccessfulResponse.StatusCode, body.Message));
            }
            catch(Exception exc)
            {
                ret = new RestException(string.Format("{0} returned {1}", UnsuccessfulResponse.ResponseUri, UnsuccessfulResponse.StatusCode), exc);
            }
            finally
            {
                ret.UnsuccessfulResponse = UnsuccessfulResponse;    
            }
            return ret;
        }

        private struct UnsuccessfulBody
        {
            public string Message;
        }
    }
}
