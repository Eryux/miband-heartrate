using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Devices.Enumeration;

using MiBand_Heartrate_2.Devices;

namespace MiBand_Heartrate_2
{
    public class ConnectionWindowViewModel : ViewModel
    {
        ObservableCollection<DeviceInformation> _devices = new ObservableCollection<DeviceInformation>();

        public ObservableCollection<DeviceInformation> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                InvokePropertyChanged("Devices");
            }
        }

        DeviceInformation _selectedDevice;

        public DeviceInformation SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                InvokePropertyChanged("SelectedDevice");
            }
        }

        DeviceModel _deviceModel = DeviceModel.MIBAND_2_3;

        public DeviceModel DeviceModel
        {
            get { return _deviceModel; }
            set
            {
                _deviceModel = value;
                InvokePropertyChanged("DeviceModel");
            }
        }


        public ConnectionWindow View { get; set; }

        public MainWindowViewModel Main { get; set; }


        BLE _bluetooth;

        // --------------------------------------

        public ConnectionWindowViewModel()
        {
            _bluetooth = new BLE(new string[] {"System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected"});

            _bluetooth.Watcher.Added += OnBluetoothAdded;
            _bluetooth.Watcher.Updated += OnBluetoothUpdated;
            _bluetooth.Watcher.Removed += OnBluetoothRemoved;

            _bluetooth.StartWatcher();
        }

        ~ConnectionWindowViewModel()
        {
            if (_bluetooth.Watcher != null)
            {
                _bluetooth.Watcher.Added -= OnBluetoothAdded;
                _bluetooth.Watcher.Updated -= OnBluetoothUpdated;
                _bluetooth.Watcher.Removed -= OnBluetoothRemoved;
            }
            
            _bluetooth.StopWatcher();
        }

        private void OnBluetoothRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            DeviceInformation[] pending = Devices.Where(x => x.Id == args.Id).ToArray();
            for (int i = 0; i < pending.Length; ++i) {
                Devices.Remove(pending[i]);
            }
        }

        private void OnBluetoothUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            foreach (DeviceInformation d in Devices)
            {
                if (d.Id == args.Id) {
                    d.Update(args);
                    break;
                }
            }
        }

        private void OnBluetoothAdded(DeviceWatcher sender, DeviceInformation args)
        {
            Devices.Add(args);
        }

        // --------------------------------------

        ICommand _command_connect;

        public ICommand Command_Connect
        {
            get
            {
                if (_command_connect == null)
                {
                    _command_connect = new RelayCommand<object>("connection.connect", "Connect to selected device", o =>
                    {
                        Device device = null;

                        switch (DeviceModel)
                        {
                            case DeviceModel.DUMMY:
                                device = new Dummy_Device();
                                break;
                            case DeviceModel.MIBAND_2_3:
                                device = new MiBand2_3_Device(SelectedDevice);
                                break;
                            case DeviceModel.MIBAND_4:
                                var authWindow = new AuthenticationKeyWindow();

                                if ((bool)authWindow.ShowDialog())
                                {
                                    device = new MiBand4_Device(SelectedDevice, authWindow.AuthenticationKeyResult);
                                }

                                break;
                        }

                        if (device != null)
                        {
                            device.Connect();
                            
                            if (Main != null)
                            {
                                Main.Device = device;
                            }

                            if (View != null)
                            {
                                View.DialogResult = true;
                                View.Close();
                            }
                        }

                    }, o => {
                        return SelectedDevice != null;
                    });
                }

                return _command_connect;
            }
        }

        ICommand _command_cancel;

        public ICommand Command_Cancel
        {
            get
            {
                if (_command_cancel == null)
                {
                    _command_cancel = new RelayCommand<object>("connection.cancel", "Cancel connection", o =>
                    {
                        if (View != null)
                        {
                            View.DialogResult = false;
                            View.Close();
                        }
                    });
                }

                return _command_cancel;
            }
        }
    }
}
