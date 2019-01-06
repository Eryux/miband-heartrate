using System;
using System.Windows.Forms;

namespace MiBand_Heartrate
{
    public partial class ControlFrame : Form
    {

        MiBand device;

        public ControlFrame()
        {
            InitializeComponent();
        }

        private void ControlFrame_Load(object sender, EventArgs e)
        {
            device = new MiBand(DeviceControl.Instance.BluetoothManager);
        }

        private void authButton_Click(object sender, EventArgs e)
        {
            device.Authenticate(OnAuth);
        }

        delegate void OnAuthHandler(MiBand d, bool s);

        void OnAuth(MiBand d, bool s)
        {
            if (InvokeRequired)
            {
                OnAuthHandler c = new OnAuthHandler(OnAuth);
                Invoke(c, new object[] { d, s });
            }
            else
            {
                if (s) {
                    connectionStatusLabel.Text = "Connected | Auth";
                    startButton.Enabled = true;
                }
                else {
                    MessageBox.Show("Auth failed !", "Device error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            device.StartMonitorHeartrate();
            device.HeartrateChanged += OnHeartrateChange;
            stopButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            device.StopMonitorHeartrate();
            device.HeartrateChanged -= OnHeartrateChange;
            heartrateLabel.Text = "--";
            startButton.Enabled = true;
        }

        delegate void OnHeartrateChangeHandler(MiBand d, UInt16 v);

        void OnHeartrateChange(MiBand d, UInt16 v)
        {
            if (InvokeRequired)
            {
                OnHeartrateChangeHandler c = new OnHeartrateChangeHandler(OnHeartrateChange);
                Invoke(c, new object[] { d, v });
            }
            else
            {
                heartrateLabel.Text = String.Format("{0} bpm", v);
            }
        }

        private void ControlFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            device.StopMonitorHeartrate();
            if (DeviceControl.Instance.BluetoothManager.HasDeviceConnected()) {
                DeviceControl.Instance.BluetoothManager.Disconnect();
            }
        }

        private void saveCSVCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (saveCSVCheckbox.Checked) {
                device.HeartrateChanged += DeviceControl.Instance.WriteDataInCSV;
            }
            else {
                device.HeartrateChanged -= DeviceControl.Instance.WriteDataInCSV;
            }
        }

        private void realtimeFileCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (realtimeFileCheck.Checked) {
                device.HeartrateChanged += DeviceControl.Instance.WriteRealtimeHeartrateInFile;
            }
            else {
                device.HeartrateChanged -= DeviceControl.Instance.WriteRealtimeHeartrateInFile;
            }
        }
    }
}
