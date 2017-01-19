using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ReadGPS
{

    public partial class frmPP : Form
    {
       
#region Member Variables

        // Local variables used to hold the present
        // position as latitude and longitude
        public string Latitude;
        public string Longitude;

#endregion
       
#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public frmPP()
        {
            InitializeComponent();
        }

#endregion

        /// <summary>
        /// Try to update present position if the port is configured correctly
        /// and the GPS device is returning values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string data = serialPort1.ReadExisting();
                string[] strArr = data.Split('$');
                foreach (string strTemp in strArr)
                {
                    string[] lineArr = strTemp.Split(',');
                    if (lineArr[0] == "GPGGA")
                    {
                        try
                        {
                            //Latitude
                            Double dLat = Convert.ToDouble(lineArr[2], CultureInfo.InvariantCulture.NumberFormat);
                            int a = (int)dLat / 100;
                            Double b = (dLat % 100) / 60;
                            Latitude = lineArr[3].ToString() + (a + b).ToString("F5", CultureInfo.InvariantCulture.NumberFormat);

                            //Longitude
                            Double dLon = Convert.ToDouble(lineArr[4], CultureInfo.InvariantCulture.NumberFormat);
                            a = (int)dLon / 100;
                            b = (dLon % 100) / 60;
                            Longitude = lineArr[5].ToString() + (a + b).ToString("F5", CultureInfo.InvariantCulture.NumberFormat);

                            //Display
                            txtLat.Text = Latitude;
                            txtLong.Text = Longitude;
                            btnMapIt.Enabled = true;
                        }
                        catch
                        {
                            //Can't Read GPS values
                            txtLat.Text = "GPS Unavailable";
                            txtLong.Text = "GPS Unavailable";
                            btnMapIt.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                txtLat.Text = "COM Port Closed";
                txtLong.Text = "COM Port Closed";
                btnMapIt.Enabled = false;
            }
        }

        /// <summary>
        /// Enable or disable the timer to start continuous
        /// updates or disable all updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
                button1.Text = "Update";
            }   
            else
            {
                timer1.Enabled = true;
                button1.Text = "Stop Updates";
            }
        }

        /// <summary>
        /// Open a map of the present position in external Firefox browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMapIt_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder queryAddress = new StringBuilder();
                queryAddress.Append("http://maps.google.com/maps?q=");
               // queryAddress.Append("http://www.openstreetmap.org/#map=16/");

                if (Latitude != string.Empty)
                    queryAddress.Append(Latitude + "%2C");
                
                if (Longitude != string.Empty)
                    queryAddress.Append(Longitude);

                Process.Start("Firefox.exe", queryAddress.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        /// <summary>
        /// Get list of available COMports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboPorts_Click(object sender, EventArgs e)
        {
            cboPorts.Items.Clear();     //Delete items in list when entered again
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
                cboPorts.Items.Add(portName);
        }

        /// <summary>
        /// Select COMport from drop down list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();  // Just in case it was already opened
                serialPort1.PortName = cboPorts.SelectedItem.ToString();
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }
    }
}