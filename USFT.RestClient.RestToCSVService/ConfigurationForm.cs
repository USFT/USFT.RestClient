using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.ServiceProcess;

namespace USFTRestToCSVService
{
    public partial class ConfigurationForm : Form
    {
        public ConfigurationForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        static void ChangeSetting(KeyValueConfigurationCollection collection, string key, string value)
        {
            if (collection.AllKeys.Contains(key))
                collection[key].Value = value;
            else
                collection.Add(key, value);
        }

        void SaveSettings()
        {
            var currentLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(currentLocation);
            var collection = config.AppSettings.Settings;
            ChangeSetting(collection, "apikey", textAPIKey.Text);
            ChangeSetting(collection, "username", textUserName.Text);
            ChangeSetting(collection, "frequency", textFrequency.Text);
            ChangeSetting(collection, "targetfile", textOutputFile.Text);
            HashSet<string> uncheckedItems = new HashSet<string>();
            foreach (var entry in clbIncludedColumns.Items)
            {
                uncheckedItems.Add(entry.ToString());
            }
            foreach(var entry in clbIncludedColumns.CheckedItems)
            {
                var name = entry.ToString();
                uncheckedItems.Remove(name);
                ChangeSetting(collection, "output" + name.ToLower(), "true");
            }
            foreach(var entry in uncheckedItems)
            {
                ChangeSetting(collection, "output" +entry.ToLower(), "false");
            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        void LoadSettings()
        {
            textAPIKey.Text = ConfigurationManager.AppSettings["apikey"];
            textUserName.Text = ConfigurationManager.AppSettings["username"];
            textFrequency.Text = ConfigurationManager.AppSettings["frequency"];
            textOutputFile.Text = ConfigurationManager.AppSettings["outputfile"];
            saveFileDialog1.FileName = textOutputFile.Text;
            string[] outputRaw = new string[]{
                ConfigurationManager.AppSettings["outputserial"],
                ConfigurationManager.AppSettings["outputname"],
                ConfigurationManager.AppSettings["outputlatitude"],
                ConfigurationManager.AppSettings["outputlongitude"],
                ConfigurationManager.AppSettings["outputheading"],
                ConfigurationManager.AppSettings["outputvelocity"],
                ConfigurationManager.AppSettings["outputsatellites"],
                ConfigurationManager.AppSettings["outputignition"],
                ConfigurationManager.AppSettings["outputlastmoved"],
                ConfigurationManager.AppSettings["outputlastupdated"],
                ConfigurationManager.AppSettings["outputflags"]
            };
            for(int i = 0;i<outputRaw.Length;i++)
            {
                bool val;
                if (!bool.TryParse(outputRaw[i], out val))
                    val = true;
                clbIncludedColumns.SetItemChecked(i, val);
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var result = saveFileDialog1.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                textOutputFile.Text = saveFileDialog1.FileName;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();
            }
            catch(Exception)
            {
                MessageBox.Show("Save failed. You may need to run this configuration utility as an Administrator.", "Save failed!", MessageBoxButtons.OK);
                return;
            }

            try
            {
                OrderServiceToRefresh();
            }
            catch(Exception)
            {
                MessageBox.Show("Settings were saved, but the service did not respond to the reload-settings command. Please manually restart the USFTRestToCSV Service.", "Reload failed!", MessageBoxButtons.OK);
                return;
            }
            this.Close();
        }

        private void OrderServiceToRefresh()
        {
            ServiceController service = new ServiceController("USFTRestToCSV");
            if(service.Status == ServiceControllerStatus.Running)
                service.ExecuteCommand(USFTRestToCSV.REFRESH_CONFIG_COMMAND);
            else
            {
                switch(service.Status)
                {
                    case ServiceControllerStatus.Stopped:
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                        OrderServiceToRefresh();
                        break;
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.StartPending:
                        service.WaitForStatus(ServiceControllerStatus.Running);
                        OrderServiceToRefresh();
                        break;
                    default:
                        return;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
