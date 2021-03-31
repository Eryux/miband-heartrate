using System;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace MiBand_Heartrate_2
{
    public class BLE : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        DeviceWatcher _watcher;

        public DeviceWatcher Watcher
        {
            get { return _watcher; }
            set
            {
                _watcher = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Watcher"));
            }
        }

        // --------------------------------------

        public BLE(string[] filters)
        {
            Watcher = DeviceInformation.CreateWatcher(
                BluetoothLEDevice.GetDeviceSelectorFromPairingState(true),
                filters,
                DeviceInformationKind.AssociationEndpoint
            );
        }

        ~BLE()
        {
            if (Watcher != null)
            {
                StopWatcher();
            }
            Watcher = null;
        }

        public void StartWatcher()
        {
            if (Watcher != null)
            {
                Watcher.Start();
            }
        }

        public void StopWatcher()
        {
            if (Watcher != null && Watcher.Status == DeviceWatcherStatus.Started)
            {
                Watcher.Stop();
            }
        }

        // --------------------------------------

        static async public void Write(GattCharacteristic characteristic, byte[] data)
        {
            using (var stream = new DataWriter())
            {
                stream.WriteBytes(data);

                try
                {
                    GattCommunicationStatus r = await characteristic.WriteValueAsync(stream.DetachBuffer());

                    if (r != GattCommunicationStatus.Success)
                    {
                        Console.WriteLine(string.Format("Unable to write on {0} - {1}", characteristic.Uuid, r));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
