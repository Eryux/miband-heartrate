using System;
using System.Threading;

namespace MiBand_Heartrate_2.Devices
{
    public class Dummy_Device : Device
    {
        bool _running = false;

        Thread _worker = null;

        public Dummy_Device() : base() 
        {
            Name = "Dummy";
        }

        public override void Dispose()
        {
            Disconnect();
        }

        public override void Authenticate()
        {
            if (Status == Devices.DeviceStatus.ONLINE_UNAUTH)
            {
                Status = Devices.DeviceStatus.ONLINE_AUTH;
            }
        }

        public override void Connect()
        {
            if (Status == Devices.DeviceStatus.OFFLINE)
            {
                Status = Devices.DeviceStatus.ONLINE_UNAUTH;
            }
        }

        public override void Disconnect()
        {
            StopHeartrateMonitor();

            if (Status != Devices.DeviceStatus.OFFLINE)
            {
                Status = Devices.DeviceStatus.OFFLINE;
            }
        }

        public override void StartHeartrateMonitor(bool continuous = false)
        {
            if ( ! _running && _worker == null)
            {
                _running = true;

                _worker = new Thread(new ThreadStart(FakeHeartrateValueWorker));
                _worker.Start();

                HeartrateMonitorStarted = true;
            }
        }

        public override void StopHeartrateMonitor()
        {
            if (_running || _worker != null)
            {
                _running = false;
                _worker = null;
            }
        }

        void FakeHeartrateValueWorker()
        {
            var rnd = new Random();
            
            try
            {

                while (_running)
                {
                    Heartrate = (ushort)rnd.Next(55, 180);
                    Thread.Sleep(3000);
                }
            }
            catch (ThreadAbortException) { }

            HeartrateMonitorStarted = false;
        }
    }
}
