using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace USFTRestToCSVService
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string [] args)
        {
            string order = null;
            if (args.Length >= 1)
                order = (args[0] ?? "").ToLower();
            
            switch(order)
            {
                case "/install":
                    PerformInstall();
                    break;
                case "/uninstall":
                    PerformUninstall();
                    break;
                case "/config":
                    OpenConfig();
                    break;
                case "/start":
                    StartInstalledService();
                    break;
                case "/service":
                case null:
                default:
                    BeginServiceExecution();
                    break;
            }
        }

        private static void StartInstalledService()
        {
            ServiceController service = new ServiceController("USFTRestToCSV");
            service.Start();
        }

        private static void PerformUninstall()
        {
            try
            {
                TransactedInstaller ti = new TransactedInstaller();
                ProjectInstaller mi = new ProjectInstaller();
                ti.Installers.Add(mi);
                String path = String.Format("/assemblypath={0}",
                System.Reflection.Assembly.GetExecutingAssembly().Location);
                String[] cmdline = { path };
                InstallContext ctx = new InstallContext("", cmdline);
                ti.Context = ctx;
                ti.Uninstall(null);
            }
            catch(Exception exc)
            {
                ExceptionHandler.Output("/uninstall failed", exc);
            }
        }

        private static void PerformInstall()
        {
            try
            {
                TransactedInstaller ti = new TransactedInstaller();
                ProjectInstaller mi = new ProjectInstaller();
                ti.Installers.Add(mi);
                String path = String.Format("/assemblypath={0}",
                  System.Reflection.Assembly.GetExecutingAssembly().Location);
                String[] cmdline = { path };
                InstallContext ctx = new InstallContext("", cmdline);
                ti.Context = ctx;
                ti.Install(new Hashtable());
            }
            catch(Exception exc)
            {
                ExceptionHandler.Output("/install failed", exc);
            }
        }

        static void BeginServiceExecution()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new USFTRestToCSV() 
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void OpenConfig()
        {
            Thread t = new Thread(ConfigFormTask);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        static void ConfigFormTask()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConfigurationForm());
        }
    }
}
