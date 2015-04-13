using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace USFTRestToCSVService
{
    public partial class USFTRestToCSV : ServiceBase
    {
        public const int REFRESH_CONFIG_COMMAND = 213;

        Downloader activeDownloader = null;

        public USFTRestToCSV()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            activeDownloader = new Downloader();

            try
            {
                activeDownloader.Start();
            }
            catch(Exception exc)
            {
                ExceptionHandler.Output("Could not initialize the Downloader service", exc);
                throw exc;
            }
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == REFRESH_CONFIG_COMMAND)
                ConfigurationManager.RefreshSection("appSettings");
        }

        protected override void OnStop()
        {
            activeDownloader.Stop();
            activeDownloader = null;
        }

        protected override void OnPause()
        {
            activeDownloader.Stop();
        }

        protected override void OnContinue()
        {
            try
            {
                activeDownloader.Start();
            }
            catch (Exception exc)
            {
                ExceptionHandler.Output("Could not initialize the Downloader service", exc);
                throw exc;
            }
        }
    }
}
