using System;
using System.IO;

namespace MiBand_Heartrate
{
    class DeviceControl
    {

        #region DeviceControl_Singleton
        private static readonly DeviceControl instance = new DeviceControl();

        static DeviceControl() { }

        private DeviceControl() { }

        public static DeviceControl Instance
        {
            get { return instance; }
        }
        #endregion

        BLEManager bluetoothManager = null;

        public BLEManager BluetoothManager { get { return bluetoothManager; } }

        public void Initialize()
        {
            bluetoothManager = new BLEManager();
        }

        public void WriteRealtimeHeartrateInFile(MiBand d, UInt16 v)
        {
            using (StreamWriter fp = new StreamWriter("heartrate.txt")) {
                fp.Write(v.ToString());
            }
        }

        public void WriteDataInCSV(MiBand d, UInt16 v)
        {
            StreamWriter fp;
            if (File.Exists("heartrates.csv") == false) {
                fp = new StreamWriter("heartrates.csv");
                fp.WriteLine("datetime,heartrate");
            }
            else {
                fp = new StreamWriter("heartrates.csv", true);
            }
            fp.WriteLine(String.Format("{0},{1}", DateTime.Now.ToString(), v.ToString()));
            fp.Close();
        }
    }
}
