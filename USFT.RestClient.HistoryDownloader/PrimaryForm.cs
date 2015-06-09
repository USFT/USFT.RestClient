using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using USFT.RestClient.v1;

namespace USFT.RestClient.HistoryDownloader
{
    public partial class PrimaryForm : Form
    {
        public PrimaryForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = saveFileDialog1.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                txtFileLocation.Text = saveFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MessageBox.Show("Please enter a user name", "Missing parameter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password", "Missing parameter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFileLocation.Text))
            {
                MessageBox.Show("Please specify a file location", "Missing parameter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Task.Factory.StartNew(PerformRequest);
        }

        private void Output(string text)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new MethodInvoker(() =>
                {
                    txtOutput.AppendText(text + "\r\n");
                }));
                Task.Delay(10).Wait(); // quick hack to make sure output actually reaches the text box, and in the correct order.
            }
            else
                txtOutput.AppendText(text + "\r\n");
        }

        private void PerformRequest()
        {
            UsftClient client = new UsftClient(txtUser.Text, txtPassword.Text, UsftClient.AuthenticationMode.Basic);
            Output("Testing connection...");
            var testResult = client.TestConnection();
            if(!testResult)
            {
                Output("Test FAILED. Please check your user name and password, and try again.");
                return;
            }
            Output("Test succeeded. Requesting and parsing data...");
            var requestResult = GetResults(client, dtpFrom.Value, dtpTo.Value);
            Output("Data retrieved and parsed. " + requestResult.Count + " rows found.");
            Output("Formatting...");
            try
            {
                Formatter.Save(requestResult, txtFileLocation.Text).Wait();
            }
            catch(Exception exc)
            {
                Output("Error encountered while saving file: " + exc.Message);
                return;
            }
            Output("File saved.");
        }

        private List<DeviceLocation> GetResults(UsftClient client, DateTime requestedStart, DateTime requestedEnd)
        {
            bool breakDownRequestByHour = false;
            if(requestedEnd - requestedStart > TimeSpan.FromHours(1))
            {
                var deviceList = client.GetDeviceLocations(); // retrieve the number of devices with recent updates
                if(breakDownRequestByHour = deviceList.Count > 50)
                {
                    Output("Large account detected! Breaking request down hour by hour. This may take a little while.");
                }
            }
            if (breakDownRequestByHour)
            {
                var start = requestedStart;
                var limit = requestedEnd.AddHours(-1);
                List<DeviceLocation> ret = new List<DeviceLocation>();
                while(start < limit)
                {
                    var end = start.AddHours(1).AddSeconds(-1);
                    Output("Requesting " + start.ToString() + " to " + end.ToString());
                    ret.AddRange(client.GetHistoryFromTo(null, start, end));
                    start = end.AddSeconds(1);
                }
                Output("Requesting " + start.ToString() + " to " + requestedEnd.ToString());
                ret.AddRange(client.GetHistoryFromTo(null, start, requestedEnd));
                return ret;
            }
            else
            {
                return client.GetHistoryFromTo(null, requestedStart, requestedEnd);
            }
        }
    }
}
