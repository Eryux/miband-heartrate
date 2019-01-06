using System;
using System.Linq;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace MiBand_Heartrate
{
    /* MiBand
     * Used for manage Mi Band 2 BLE device - Authenticate and manager heartrate sensor
     */
    class MiBand
    {
        // Services UUID
        public static string ATS_UUID = "0000fee1-0000-1000-8000-00805f9b34fb"; // Auth
        public static string HMS_UUID = "0000180d-0000-1000-8000-00805f9b34fb"; // Heartrate
        public static string SNS_UUID = "0000fee0-0000-1000-8000-00805f9b34fb"; // Sensor

        // Characteristics UUID
        public static string ATC_UUID = "00000009-0000-3512-2118-0009af100700"; // Auth
        public static string HRM_UUID = "00002a37-0000-1000-8000-00805f9b34fb"; // Heartrate notify
        public static string HMC_UUID = "00002a39-0000-1000-8000-00805f9b34fb"; // Heartrate control
        public static string SNC_UUID = "00000001-0000-3512-2118-0009af100700"; // Sensor control

        // ----------------------------

        public delegate void OnHeartrateChangedHandler(MiBand device, UInt16 value);

        public delegate void OnAuthHandler(MiBand device, bool status);

        BLEManager manager;

        byte[] _authKey;

        GattDeviceService HMSService = null;
        GattCharacteristic HMCCharacteristic = null;
        GattCharacteristic SNSCharacteristic = null;

        Thread pingThread = null;

        public event OnHeartrateChangedHandler HeartrateChanged;

        // ----------------------------

        public MiBand(BLEManager bluetoothLEManager) { manager = bluetoothLEManager; }

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
        async public void Authenticate(OnAuthHandler callback = null)
        {
            GattDeviceServicesResult servicesResult = await manager.Device.GetGattServicesForUuidAsync(new Guid(ATS_UUID));
            if (servicesResult.Status == GattCommunicationStatus.Success && servicesResult.Services.Count > 0)
            {
                GattDeviceService service = servicesResult.Services[0];

                GattCharacteristicsResult characteristicsResult = await service.GetCharacteristicsForUuidAsync(new Guid(ATC_UUID));
                if (characteristicsResult.Status == GattCommunicationStatus.Success && characteristicsResult.Characteristics.Count > 0)
                {
                    GattCharacteristic characteristic = characteristicsResult.Characteristics[0];

                    // Enable notification
                    GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                    if (status == GattCommunicationStatus.Success)
                    {
                        characteristic.ValueChanged += (GattCharacteristic sender, GattValueChangedEventArgs args) => {
                            var reader = DataReader.FromBuffer(args.CharacteristicValue);
                            byte[] messageType = new byte[3]; reader.ReadBytes(messageType);

                            if (messageType[1] == 0x01) // Request random number
                            {
                                if (messageType[2] == 0x01) {
                                    manager.Write(sender, new byte[] { 0x02, 0x08 });
                                } else {
                                    callback.Invoke(this, false);
                                    service.Dispose();
                                }
                            }
                            else if (messageType[1] == 0x02) // Get random number
                            {
                                byte[] number = new byte[reader.UnconsumedBufferLength];
                                reader.ReadBytes(number);

                                // Encrypt number
                                byte[] encNumber;
                                using (Aes aes = Aes.Create()) {
                                    aes.Key = _authKey;
                                    aes.Mode = CipherMode.ECB;
                                    aes.Padding = PaddingMode.None;

                                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }); // Constant iv, bad :(
                                    using (MemoryStream stream = new MemoryStream()) {
                                        using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write)) {
                                            cryptoStream.Write(number, 0, number.Length);
                                            cryptoStream.FlushFinalBlock();
                                            encNumber = stream.ToArray();
                                        }
                                    }
                                }

                                // Send encrypted number
                                using (var stream = new MemoryStream()) {
                                    stream.Write(new byte[] { 0x03, 0x08 }, 0, 2);
                                    stream.Write(encNumber, 0, encNumber.Length);
                                    manager.Write(characteristic, stream.ToArray());
                                }
                            }
                            else if (messageType[1] == 0x03)
                            {
                                if (messageType[2] == 0x01) {
                                    callback.Invoke(this, true);
                                } else {
                                    callback.Invoke(this, false);
                                    service.Dispose();
                                }
                            }
                        };
                    }

                    // Define auth secret key
                    byte[] hash = new SHA256Managed().ComputeHash(Guid.NewGuid().ToByteArray());
                    _authKey = hash.Take(16).ToArray();

                    // Send key it to device
                    using (var stream = new MemoryStream()) {
                        stream.Write(new byte[] { 0x01, 0x08 }, 0, 2);
                        stream.Write(_authKey, 0, _authKey.Length);
                        manager.Write(characteristic, stream.ToArray());
                    }
                }
            }
        }

        /* Mi Band 2 - Continious heartrate monitoring
         *  1. Enable sensor
         *  2. Enable notification on heartrate characteristic
         *  3. Start heartrate sensor on continious measuring
         *  4. Activate sensor characteristic with 0x02
         *  5. Send ping 0x16 after each heartrate notice to keep sensor active
         *  see https://medium.com/machine-learning-world/how-i-hacked-xiaomi-miband-2-to-control-it-from-linux-a5bd2f36d3ad
         */
        async public void StartMonitorHeartrate()
        {
            // Enabling sensor
            GattDeviceServicesResult servicesResult = await manager.Device.GetGattServicesForUuidAsync(new Guid(SNS_UUID));
            if (servicesResult.Status == GattCommunicationStatus.Success && servicesResult.Services.Count > 0)
            {
                GattDeviceService service = servicesResult.Services[0];
                GattCharacteristicsResult characteristicsResult = await service.GetCharacteristicsForUuidAsync(new Guid(SNC_UUID));
                if (characteristicsResult.Status == GattCommunicationStatus.Success && characteristicsResult.Characteristics.Count > 0)
                {
                    GattCharacteristic characteristic = characteristicsResult.Characteristics[0];
                    SNSCharacteristic = characteristic;
                    manager.Write(characteristic, new byte[] { 0x01, 0x03, 0x19 });
                }
            }

            // Enabling Heartrate measurements
            servicesResult = await manager.Device.GetGattServicesForUuidAsync(new Guid(HMS_UUID));
            if (servicesResult.Status == GattCommunicationStatus.Success && servicesResult.Services.Count > 0)
            {
                GattDeviceService service = servicesResult.Services[0];

                // Enabling notification
                GattCharacteristicsResult characteristicsResult = await service.GetCharacteristicsForUuidAsync(new Guid(HRM_UUID));
                if (characteristicsResult.Status == GattCommunicationStatus.Success && characteristicsResult.Characteristics.Count > 0)
                {
                    GattCharacteristic characteristic = characteristicsResult.Characteristics[0];
                    GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                    if (status == GattCommunicationStatus.Success)
                    {
                        characteristic.ValueChanged += (GattCharacteristic sender, GattValueChangedEventArgs args) => {
                            var reader = DataReader.FromBuffer(args.CharacteristicValue);
                            UInt16 heartrate = reader.ReadUInt16();
                            HeartrateChanged?.Invoke(this, heartrate);
                        };
                    }
                }

                // Enable continious measurements
                characteristicsResult = await service.GetCharacteristicsForUuidAsync(new Guid(HMC_UUID));
                if (characteristicsResult.Status == GattCommunicationStatus.Success && characteristicsResult.Characteristics.Count > 0)
                {
                    GattCharacteristic characteristic = characteristicsResult.Characteristics[0];
                    HMCCharacteristic = characteristic;

                    manager.Write(characteristic, new byte[] { 0x15, 0x01, 0x01 });

                    if (SNSCharacteristic != null) {
                        manager.Write(SNSCharacteristic, new byte[] { 0x02 });
                    }
                }

                // Enable ping HMC every 10 sec.
                pingThread = new Thread(new ThreadStart(RunPingSensor));
                pingThread.Start();

                HMSService = service;
            }
        }

        public void StopMonitorHeartrate()
        {
            if (HMCCharacteristic != null) {
                manager.Write(HMCCharacteristic, new byte[] { 0x15, 0x01, 0x00 });
                HMCCharacteristic = null;
            }

            if (HMSService != null) {
                HMSService.Dispose();
                HMSService = null;
            }

            if (pingThread != null) {
                pingThread.Join();
                pingThread = null;
            }
        }

        void RunPingSensor()
        {
            while (HMCCharacteristic != null){
                manager.Write(HMCCharacteristic, new byte[] { 0x16 });
                Thread.Sleep(10000);
            }
        }
    }
}
