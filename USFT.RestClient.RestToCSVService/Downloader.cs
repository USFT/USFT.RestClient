using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USFT.RestClient.v1;
using System.Configuration;

namespace USFTRestToCSVService
{
    class Downloader
    {
        #region Parameters
        int SecondsBetweenRetrievals
        {
            get
            {
                int ret;
                if (!int.TryParse(ConfigurationManager.AppSettings["frequency"], out ret))
                    ret = DEFAULT_SECONDS_BETWEEN_DOWNLOADS;
                return ret;
            }
        }

        string UserName
        {
            get
            {
                return ConfigurationManager.AppSettings["username"];
            }
        }

        string APIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["apikey"];
            }
        }
        #endregion

        /// <summary>
        /// this sets the upper limit on how long the task takes to responds to a request to pause or stop
        /// </summary>
        const int MILLISECONDS_BETWEEN_ITERATION_CHECKS = 200;
        const int DEFAULT_SECONDS_BETWEEN_DOWNLOADS = 60;
        bool working = false;
        Task downloadTask = null;

        public Downloader()
        {
        }

        public void Start()
        {
            working = true;

            if(downloadTask == null)
            {
                downloadTask = PrimaryLoop();
            }
        }

        public void Stop()
        {
            working = false;
            if(downloadTask != null)
            {
                downloadTask.Wait();
                downloadTask = null;
            }
        }

        async Task PrimaryLoop()
        {
            while(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(APIKey))
            {
                if(!working)
                    return;
                await Task.Delay(MILLISECONDS_BETWEEN_ITERATION_CHECKS);
                ConfigurationManager.RefreshSection("appSettings");
            }

            var client = new RestClient(UserName, APIKey);
            DateTime lastRetrieval;
            TimeSpan betweenRetrievals = TimeSpan.FromSeconds(SecondsBetweenRetrievals);
            while(working)
            {
                lastRetrieval = DateTime.UtcNow;
                List<DeviceLocation> results = null;

                try
                {
                    results = client.GetDeviceLocations();
                }
                catch(RestException exc)
                {
                    ExceptionHandler.Output("The RestClient could not retrieve the device location data", exc);
                }
                catch(Exception exc)
                {
                    ExceptionHandler.Output("The RestClient encountered an unexpected exception while retrieving device location data", exc);
                }

                if (results != null)
                {
                    await Formatter.Save(results);
                }

                while ((DateTime.UtcNow - lastRetrieval) < betweenRetrievals && working)
                    await Task.Delay(MILLISECONDS_BETWEEN_ITERATION_CHECKS);
            }
        }
    }
}
