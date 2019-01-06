using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace MiBand_Heartrate
{
    /* BLEManager
     * Used for manage and discover Bluetooth LE device
     * see https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/gatt-client
     */
    class BLEManager
    {
        public delegate void OnDeviceConnected(BLEManager manager);


        DeviceWatcher watcher = null;

        List<DeviceInformation> devices = new List<DeviceInformation>();

        BluetoothLEDevice connectedDevice = null;


        public BluetoothLEDevice Device { get { return connectedDevice; } }

        public DeviceWatcher CreateWatcher()
        {
            if (watcher == null)
            {
                devices.Clear();
                string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
                watcher = DeviceInformation.CreateWatcher(
                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(true),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint
                );

                watcher.Added += (DeviceWatcher w, DeviceInformation d) => {
                    devices.Add(d);
                };

                watcher.Updated += (DeviceWatcher w, DeviceInformationUpdate d) => {
                    foreach (DeviceInformation device in devices) {
                        if (device.Id == d.Id) { device.Update(d); break; }
                    }
                };

                watcher.Removed += (DeviceWatcher w, DeviceInformationUpdate d) => {
                    foreach (DeviceInformation device in devices) {
                        if (device.Id == d.Id) { devices.Remove(device); break; }
                    }
                };
            }

            return watcher;
        }

        public void StartWatcher() { watcher.Start(); }

        public void StopWatcher() { watcher.Stop(); }

        public async void Connect(string deviceId, OnDeviceConnected callback = null)
        {
            connectedDevice = await BluetoothLEDevice.FromIdAsync(deviceId);
            callback.Invoke(this);
        }

        public void Disconnect()
        {
            if (connectedDevice != null) { connectedDevice.Dispose(); connectedDevice = null; }
        }

        public bool IsWatchingForDevice()
        {
            return (watcher != null && watcher.Status == DeviceWatcherStatus.Started);
        }

        public bool HasDeviceConnected()
        {
            return (connectedDevice != null);
        }

        async public void Write(GattCharacteristic c, byte[] data)
        {
            if ( ! HasDeviceConnected()) { return; }

            using (var stream = new DataWriter())
            {
                stream.WriteBytes(data);

                try {
                    GattCommunicationStatus result = await c.WriteValueAsync(stream.DetachBuffer());
                    if (result != GattCommunicationStatus.Success) {
                        Console.WriteLine(String.Format("Write {0} on {1} failed !", BitConverter.ToString(data), c.Uuid));
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // Not used but can be usefull for debugging
        public async void EnumeratingUsages()
        {
            if (!HasDeviceConnected()) { return; }

            GattDeviceServicesResult services = await connectedDevice.GetGattServicesAsync();
            if (services.Status == GattCommunicationStatus.Success)
            {
                foreach (GattDeviceService s in services.Services)
                {
                    await s.RequestAccessAsync();
                    GattCharacteristicsResult characteristicsResult = await s.GetCharacteristicsAsync();

                    Console.WriteLine("Service : " + s.Uuid);
                    if (characteristicsResult.Status == GattCommunicationStatus.Success)
                    {
                        foreach (GattCharacteristic c in characteristicsResult.Characteristics)
                        {
                            GattCharacteristicProperties props = c.CharacteristicProperties;
                            Console.WriteLine("\t characteristics : " + c.Uuid + " / " + c.UserDescription);
                            if (props.HasFlag(GattCharacteristicProperties.Read))
                            {
                                Console.WriteLine("\t\tRead");
                            }
                            if (props.HasFlag(GattCharacteristicProperties.Write))
                            {
                                Console.WriteLine("\t\tWrite");
                            }
                            if (props.HasFlag(GattCharacteristicProperties.Notify))
                            {
                                Console.WriteLine("\t\tNotify");
                            }
                        }
                    }

                    s.Dispose();
                }
            }
        }
    }
}
