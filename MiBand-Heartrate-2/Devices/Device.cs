using System.ComponentModel;

namespace MiBand_Heartrate_2.Devices
{
    public enum DeviceStatus { OFFLINE, ONLINE_UNAUTH, ONLINE_AUTH }

    public enum DeviceModel {
        [Description("Hidden")]
        DUMMY,

        [Description("Mi Band 2")]
        MIBAND_2,
        
        [Description("Mi Band 4")]
        MIBAND_4
    }

    public abstract class Device : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        string _name = "";

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        DeviceStatus _status = Devices.DeviceStatus.OFFLINE;

        public DeviceStatus Status
        {
            get { return _status; }
            internal set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }


        public DeviceModel Model { get; internal set; }


        ushort _heartrate = 0;

        public ushort Heartrate
        {
            get { return _heartrate; }
            internal set
            {
                _heartrate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heartrate"));
            }
        }


        bool _heartrateMonitorStarted = false;

        public bool HeartrateMonitorStarted
        {
            get { return _heartrateMonitorStarted; }
            internal set
            {
                _heartrateMonitorStarted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeartrateMonitorStarted"));
            }
        }

        public object DeviceStatus { get; internal set; }

        // --------------------------------------

        public Device() { }

        public abstract void Dispose();

        public abstract void Connect();

        public abstract void Disconnect();

        public abstract void Authenticate();

        public abstract void StartHeartrateMonitor(bool continuous = false);

        public abstract void StopHeartrateMonitor();
    }
}
