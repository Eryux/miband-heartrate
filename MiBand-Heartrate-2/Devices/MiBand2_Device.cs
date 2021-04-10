using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace MiBand_Heartrate_2.Devices
{
    public class MiBand2_Device : Device
    {
        const string AUTH_SRV_ID = "0000fee1-0000-1000-8000-00805f9b34fb";
        const string AUTH_CHAR_ID = "00000009-0000-3512-2118-0009af100700";

        const string HEARTRATE_SRV_ID = "0000180d-0000-1000-8000-00805f9b34fb";
        const string HEARTRATE_CHAR_ID = "00002a39-0000-1000-8000-00805f9b34fb";
        const string HEARTRATE_NOTIFY_CHAR_ID = "00002a37-0000-1000-8000-00805f9b34fb";

        const string SENSOR_SRV_ID = "0000fee0-0000-1000-8000-00805f9b34fb";
        const string SENSOR_CHAR_ID = "00000001-0000-3512-2118-0009af100700";

        // --------------------------------------

        byte[] _key;

        BluetoothLEDevice _connectedDevice;


        GattDeviceService _heartrateService = null;

        GattCharacteristic _heartrateCharacteristic = null;

        GattCharacteristic _heartrateNotifyCharacteristic = null;

        GattDeviceService _sensorService = null;



        Thread _keepHeartrateAliveThread = null;

        bool _continuous = false;

        string _deviceId = "";


        public MiBand2_Device(DeviceInformation d)
        {
            _deviceId = d.Id;

            Name = d.Name;
            Model = DeviceModel.MIBAND_2;
        }


        /* Mi Band 2 - Auth
         * Before using Mi Band 2, an auth is required. Authenticate following this steps :
         *  1. Enable notification on authentification characteristic
         *  2. Generate auth key 16 bytes long
         *  3. Send key to auth characteristic and asking for a random number
         *  4. Get random number from characteristic notification
         *  5. Encrypt number using AES ECB mode without padding and with auth key
         *  6. Send encrypted random number to auth characteristic
         *  see https://leojrfs.github.io/writing/miband2-part1-auth/#reference
         */
        public override void Authenticate()
        {
            var task = Task.Run(async () =>
            {
                GattDeviceServicesResult service = await _connectedDevice.GetGattServicesForUuidAsync(new Guid(AUTH_SRV_ID));

                if (service.Status == GattCommunicationStatus.Success && service.Services.Count > 0)
                {
                    GattCharacteristicsResult characteristic = await service.Services[0].GetCharacteristicsForUuidAsync(new Guid(AUTH_CHAR_ID));

                    if (characteristic.Status == GattCommunicationStatus.Success && characteristic.Characteristics.Count > 0)
                    {
                        GattCommunicationStatus notify = await characteristic.Characteristics[0].WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        if (notify == GattCommunicationStatus.Success)
                        {
                            characteristic.Characteristics[0].ValueChanged += OnAuthenticateNotify;

                            _key = new SHA256Managed().ComputeHash(Guid.NewGuid().ToByteArray()).Take(16).ToArray();

                            using (var stream = new MemoryStream())
                            {
                                stream.Write(new byte[] { 0x01, 0x08 }, 0, 2);
                                stream.Write(_key, 0, _key.Length);
                                BLE.Write(characteristic.Characteristics[0], stream.ToArray());
                            }
                        }
                    }
                }
            });
        }

        void OnAuthenticateNotify(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] headers = new byte[3];
            
            using (DataReader reader = DataReader.FromBuffer(args.CharacteristicValue))
            {
                reader.ReadBytes(headers);

                if (headers[1] == 0x01)
                {
                    if (headers[2] == 0x01)
                    {
                        BLE.Write(sender, new byte[] { 0x02, 0x08 });
                    }
                    else
                    {
                        Extras.MessageWindow.ShowError("Authentication failed (1)");
                    }
                }
                else if (headers[1] == 0x02)
                {
                    byte[] number = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(number);

                    using (var stream = new MemoryStream())
                    {
                        stream.Write(new byte[] { 0x03, 0x08 }, 0, 2);

                        byte[] encryptedNumber = EncryptAuthenticationNumber(number);
                        stream.Write(encryptedNumber, 0, encryptedNumber.Length);

                        BLE.Write(sender, stream.ToArray());
                    }
                }
                else if (headers[1] == 0x03)
                {
                    if (headers[2] == 0x01)
                    {
                        Status = Devices.DeviceStatus.ONLINE_AUTH;
                    }
                    else
                    {
                        Extras.MessageWindow.ShowError("Authentication failed (3)");
                    }
                }
            }
        }

        byte[] EncryptAuthenticationNumber(byte[] number)
        {
            byte[] r;

            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(number, 0, number.Length);
                        cryptoStream.FlushFinalBlock();
                        r = stream.ToArray();
                    }
                }
            }

            return r;
        }


        public override void Connect()
        {
            Disconnect();

            if (_connectedDevice == null)
            {
                var task = Task.Run(async () => await BluetoothLEDevice.FromIdAsync(_deviceId));
                
                _connectedDevice = task.Result;
                _connectedDevice.ConnectionStatusChanged += OnDeviceConnectionChanged;

                Status = Devices.DeviceStatus.ONLINE_UNAUTH;

                Authenticate();
            }
        }

        private void OnDeviceConnectionChanged(BluetoothLEDevice sender, object args)
        {
            if (_connectedDevice != null && _connectedDevice.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                Status = Devices.DeviceStatus.OFFLINE;
            }
        }

        public override void Disconnect()
        {
            StopHeartrateMonitor();

            if (_connectedDevice != null)
            {
                _connectedDevice.Dispose();
                _connectedDevice = null;
            }

            Status = Devices.DeviceStatus.OFFLINE;
        }

        public override void Dispose()
        {
            Disconnect();
        }


        public override void StartHeartrateMonitor(bool continuous = false)
        {
            if (HeartrateMonitorStarted)
                return;

            _continuous = continuous;

            var task = Task.Run(async () =>
            {
                GattCharacteristic sensorCharacteristic = null;

                GattDeviceServicesResult sensorService = await _connectedDevice.GetGattServicesForUuidAsync(new Guid(SENSOR_SRV_ID));

                if (sensorService.Status == GattCommunicationStatus.Success && sensorService.Services.Count > 0)
                {
                    _sensorService = sensorService.Services[0];

                    GattCharacteristicsResult characteristic = await _sensorService.GetCharacteristicsForUuidAsync(new Guid(SENSOR_CHAR_ID));

                    if (characteristic.Status == GattCommunicationStatus.Success && characteristic.Characteristics.Count > 0)
                    {
                        sensorCharacteristic = characteristic.Characteristics[0];
                        BLE.Write(sensorCharacteristic, new byte[] { 0x01, 0x03, 0x19 });
                    }
                }

                GattDeviceServicesResult heartrateService = await _connectedDevice.GetGattServicesForUuidAsync(new Guid(HEARTRATE_SRV_ID));

                if (heartrateService.Status == GattCommunicationStatus.Success && heartrateService.Services.Count > 0)
                {
                    _heartrateService = heartrateService.Services[0];

                    GattCharacteristicsResult heartrateNotifyCharacteristic = await _heartrateService.GetCharacteristicsForUuidAsync(new Guid(HEARTRATE_NOTIFY_CHAR_ID));

                    if (heartrateNotifyCharacteristic.Status == GattCommunicationStatus.Success && heartrateNotifyCharacteristic.Characteristics.Count > 0)
                    {
                        GattCommunicationStatus notify = await heartrateNotifyCharacteristic.Characteristics[0].WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                        if (notify == GattCommunicationStatus.Success)
                        {
                            _heartrateNotifyCharacteristic = heartrateNotifyCharacteristic.Characteristics[0];
                            _heartrateNotifyCharacteristic.ValueChanged += OnHeartrateNotify;
                        }
                    }

                    GattCharacteristicsResult heartrateCharacteristicResult = await _heartrateService.GetCharacteristicsForUuidAsync(new Guid(HEARTRATE_CHAR_ID));

                    if (heartrateCharacteristicResult.Status == GattCommunicationStatus.Success && heartrateCharacteristicResult.Characteristics.Count > 0)
                    {
                        _heartrateCharacteristic = heartrateCharacteristicResult.Characteristics[0];

                        if (_continuous)
                        {
                            BLE.Write(_heartrateCharacteristic, new byte[] { 0x15, 0x01, 0x01 });

                            _keepHeartrateAliveThread = new Thread(new ThreadStart(RunHeartrateKeepAlive));
                            _keepHeartrateAliveThread.Start();
                        }
                        else
                        {
                            BLE.Write(_heartrateCharacteristic, new byte[] { 0x15, 0x02, 0x01 });
                        }

                        if (sensorCharacteristic != null)
                        {
                            BLE.Write(sensorCharacteristic, new byte[] { 0x02 });
                        }
                    }
                }

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    HeartrateMonitorStarted = true;
                });
            });
        }

        public override void StopHeartrateMonitor()
        {
            if (!HeartrateMonitorStarted)
                return;

            if (_keepHeartrateAliveThread != null)
            {
                _keepHeartrateAliveThread.Abort();
                _keepHeartrateAliveThread = null;
            }

            if (_heartrateCharacteristic != null)
            {
                BLE.Write(_heartrateCharacteristic, new byte[] { 0x15, 0x01, 0x00 });
                BLE.Write(_heartrateCharacteristic, new byte[] { 0x15, 0x02, 0x00 });
            }

            _heartrateCharacteristic = null;

            _heartrateNotifyCharacteristic = null;

            if (_heartrateService != null)
            {
                _heartrateService.Dispose();
                _heartrateService = null;
            }

            if (_sensorService != null)
            {
                _sensorService.Dispose();
                _sensorService = null;
            }

            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                HeartrateMonitorStarted = false;
            });

            GC.Collect();
        }


        void OnHeartrateNotify(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            using (DataReader reader = DataReader.FromBuffer(args.CharacteristicValue))
            {
                ushort value = reader.ReadUInt16();

                if (value > 0) // when sensor fail to retrieve heartrate it send a 0 value
                {
                    Heartrate = value;
                }

                if ( ! _continuous)
                {
                    StopHeartrateMonitor();
                }
            }
        }


        void RunHeartrateKeepAlive()
        {
            try
            {
                while (_heartrateCharacteristic != null)
                {
                    BLE.Write(_heartrateCharacteristic, new byte[] { 0x16 });
                    Thread.Sleep(5000);
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
