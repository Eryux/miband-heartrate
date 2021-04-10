using System;
using System.ComponentModel;
using System.Windows.Input;

using MiBand_Heartrate_2.Extras;

namespace MiBand_Heartrate_2
{
    public class MainWindowViewModel : ViewModel
    {
        Devices.Device _device = null;

        public Devices.Device Device
        {
            get { return _device; }
            set
            {
                if (_device != null)
                {
                    _device.PropertyChanged -= OnDevicePropertyChanged;
                    _device.Dispose();
                }

                _device = value;

                if (_device != null)
                {
                    _device.PropertyChanged += OnDevicePropertyChanged;
                }

                DeviceUpdate();

                InvokePropertyChanged("Device");
            }
        }

        bool _isConnected = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                InvokePropertyChanged("IsConnected");
            }
        }

        string _statusText = "No device connected";

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                InvokePropertyChanged("StatusText");
            }
        }

        bool _continuousMode = true;

        public bool ContinuousMode
        {
            get { return _continuousMode; }
            set
            {
                _continuousMode = value;

                Setting.Set("ContinuousMode", _continuousMode);

                InvokePropertyChanged("ContinuousMode");
            }
        }

        bool _enableFileOutput = false;

        public bool EnableFileOutput
        {
            get { return _enableFileOutput; }
            set
            {
                _enableFileOutput = value;

                Setting.Set("FileOutput", _enableFileOutput);

                InvokePropertyChanged("EnableFileOutput");
            }
        }

        bool _enableCSVOutput = false;

        public bool EnableCSVOutput
        {
            get { return _enableCSVOutput; }
            set
            {
                _enableCSVOutput = value;

                Setting.Set("CSVOutput", _enableCSVOutput);

                InvokePropertyChanged("EnableCSVOutput");
            }
        }

        bool _guard = false;

        DeviceHeartrateFileOutput _fileOutput = null;

        DeviceHeartrateCSVOutput _csvOutput = null;

        // --------------------------------------

        public MainWindowViewModel()
        {
            ContinuousMode = Setting.Get("ContinuousMode", true);
            EnableFileOutput = Setting.Get("FileOutput", false);
            EnableCSVOutput = Setting.Get("CSVOutput", false);
        }

        ~MainWindowViewModel()
        {
            Device = null;
        }

        void UpdateStatusText()
        {
            if (Device != null)
            {
                switch (Device.Status)
                {
                    case Devices.DeviceStatus.OFFLINE:
                        StatusText = "No device connected";
                        break;
                    case Devices.DeviceStatus.ONLINE_UNAUTH:
                        StatusText = string.Format("Connected to {0} | Not auth", Device.Name);
                        break;
                    case Devices.DeviceStatus.ONLINE_AUTH:
                        StatusText = string.Format("Connected to {0} | Auth", Device.Name);
                        break;
                }
            }
            else
            {
                StatusText = "No device connected";
            }
        }

        private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    DeviceUpdate();
                });

                if (Device != null)
                {
                    // Connection lost, we try to re-connect
                    if (Device.Status == Devices.DeviceStatus.OFFLINE && _guard)
                    {
                        _guard = false;
                        Device.Connect();
                    }
                    else if (Device.Status != Devices.DeviceStatus.OFFLINE)
                    {
                        _guard = true;
                    }
                }
            }
            else if (e.PropertyName == "HeartrateMonitorStarted")
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DeviceUpdate()
        {
            if (Device != null)
            {
                IsConnected = Device.Status != Devices.DeviceStatus.OFFLINE;
            }

            UpdateStatusText();
            CommandManager.InvalidateRequerySuggested();
        }

        // --------------------------------------

        ICommand _command_connect;

        public ICommand Command_Connect
        {
            get
            {
                if (_command_connect == null)
                {
                    _command_connect = new RelayCommand<object>("connect", "Connect to a device", o =>
                    {
                        var dialog = new ConnectionWindow(this);
                        dialog.ShowDialog();
                    }, o =>
                    {
                        return Device == null || Device.Status == Devices.DeviceStatus.OFFLINE;
                    });
                }

                return _command_connect;
            }
        }

        ICommand _command_disconnect;

        public ICommand Command_Disconnect
        {
            get
            {
                if (_command_disconnect == null)
                {
                    _command_disconnect = new RelayCommand<object>("disconnect", "Disconnect form connect device", o =>
                    {
                        if (Device != null)
                        {
                            _guard = false;
                            Device.Disconnect();
                            Device = null;
                        } 
                        
                        Device = null;
                    }, o =>
                    {
                        return Device != null && Device.Status != Devices.DeviceStatus.OFFLINE;
                    });
                }

                return _command_disconnect;
            }
        }

        ICommand _command_start;

        public ICommand Command_Start
        {
            get
            {
                if (_command_start == null)
                {
                    _command_start = new RelayCommand<object>("device.start", "Start heartrate monitoring", o =>
                    {
                        Device.StartHeartrateMonitor(ContinuousMode);

                        if (_enableFileOutput)
                        {
                            _fileOutput = new DeviceHeartrateFileOutput("heartrate.txt", Device);
                        }

                        if (_enableCSVOutput)
                        {
                            _csvOutput = new DeviceHeartrateCSVOutput("heartrate.csv", Device);
                        }

                    }, o =>
                    {
                        return Device != null && Device.Status == Devices.DeviceStatus.ONLINE_AUTH && !Device.HeartrateMonitorStarted;
                    });
                }

                return _command_start;
            }
        }

        ICommand _command_stop;

        public ICommand Command_Stop
        {
            get
            {
                if (_command_stop == null)
                {
                    _command_stop = new RelayCommand<object>("device.stop", "Stop heartrate monitoring", o =>
                    {
                        Device.StopHeartrateMonitor();

                        _fileOutput = null;
                        _csvOutput = null;
                    }, o =>
                    {
                        return Device != null && Device.HeartrateMonitorStarted;
                    });
                }

                return _command_stop;
            }
        }
    }
}
