using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Windows.Devices.Enumeration;

namespace MiBand_Heartrate
{
    public partial class ConnectionFrame : Form
    {
        DeviceWatcher BLEWatcher;

        Dictionary<string, string> devices;

        ControlFrame controlFrame;

        public ConnectionFrame()
        {
            InitializeComponent();
        }

        private void ConnectionFrame_Load(object sender, EventArgs e)
        {
            devices = new Dictionary<string, string>();

            DeviceControl controller = DeviceControl.Instance;
            BLEWatcher = controller.BluetoothManager.CreateWatcher();
            BLEWatcher.Added += OnDeviceAdded;
            BLEWatcher.Removed += OnDeviceRemoved;
            BLEWatcher.Start();
        }

        delegate void OnDeviceAddedHandler(DeviceWatcher w, DeviceInformation d);

        void OnDeviceAdded(DeviceWatcher w, DeviceInformation d)
        {
            if (InvokeRequired)
            {
                OnDeviceAddedHandler c = new OnDeviceAddedHandler(OnDeviceAdded);
                Invoke(c, new object[] { w, d });
            }
            else
            {
                try
                {
                    devices.Add(d.Id, d.Name);
                    deviceList.DataSource = new BindingSource(devices, null);
                    deviceList.DisplayMember = "Value";
                    deviceList.ValueMember = "Key";
                }
                catch (Exception) { }
            }
        }

        delegate void OnDeviceRemovedHandler(DeviceWatcher w, DeviceInformationUpdate d);

        void OnDeviceRemoved(DeviceWatcher w, DeviceInformationUpdate d)
        {
            if (InvokeRequired)
            {
                OnDeviceRemovedHandler c = new OnDeviceRemovedHandler(OnDeviceRemoved);
                Invoke(c, new object[] { w, d });
            }
            else
            {
                devices.Remove(d.Id);

                try {
                    deviceList.DataSource = new BindingSource(devices, null);
                    deviceList.DisplayMember = "Value";
                    deviceList.ValueMember = "Key";
                }
                catch (Exception) { }
            }
        }

        private void ConnectionFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DeviceControl.Instance.BluetoothManager.IsWatchingForDevice()) {
                DeviceControl.Instance.BluetoothManager.StopWatcher();
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;

            BLEWatcher.Stop();

            string selectedDeviceId = deviceList.SelectedValue.ToString();
            DeviceControl.Instance.BluetoothManager.Connect(selectedDeviceId, OnDeviceConnected);
        }

        delegate void OnDeviceConnectedHandler(BLEManager con);

        void OnDeviceConnected(BLEManager con)
        {
            if (InvokeRequired)
            {
                OnDeviceConnectedHandler c = new OnDeviceConnectedHandler(OnDeviceConnected);
                Invoke(c, new object[] { con });
            }
            else
            {
                if (DeviceControl.Instance.BluetoothManager.HasDeviceConnected())
                {
                    Hide();
                    controlFrame = new ControlFrame();
                    controlFrame.Closed += (s, args) => Close();
                    controlFrame.Show();
                }
                else
                {
                    MessageBox.Show("Connection to the device failed !", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connectButton.Enabled = true;
                    devices.Clear();
                    BLEWatcher.Start();
                }
            }
        }
    }
}
