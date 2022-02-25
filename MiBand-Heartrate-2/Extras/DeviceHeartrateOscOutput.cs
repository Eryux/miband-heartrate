using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MiBand_Heartrate_2.Devices;
using Rug.Osc;

namespace MiBand_Heartrate_2.Extras
{
    public class DeviceHeartrateOscOutput
    {
        Device _device;

        private OscSender _oscSender;

        private CancellationTokenSource _cancellationTokenSource;

        private static class Addresses
        {
            private const string ParametersAddress = "/avatar/parameters";

            /// <summary>
            /// Heart rate par min [0, 255]
            /// </summary>
            public static readonly string HeartRateInt = $"{ParametersAddress}/HeartRateInt";

            /// <summary>
            /// Normalized Heart rate ([0, 255] -> [0, 1])
            /// </summary>
            public static readonly string HeartRateFloat = $"{ParametersAddress}/HeartRateFloat";

            /// <summary>
            /// 1 : QRS Interval (Temporarily set it to 1/5 of the RR interval)
            /// 0 : Other times
            /// </summary>
            public static readonly string HeartBeatInt = $"{ParametersAddress}/HeartBeatInt";

            /// <summary>
            /// true : QRS Interval (Temporarily set it to 1/5 of the RR interval)
            /// false : Other times
            /// </summary>
            public static readonly string HeartBeatPulse = $"{ParametersAddress}/HeartBeatPulse";

            /// <summary>
            /// Reverses with each heartbeat
            /// </summary>
            public static readonly string HeartBeatToggle = $"{ParametersAddress}/HeartBeatToggle";
        }


        private bool _currentBeatToggle;

        public DeviceHeartrateOscOutput(Device device)
        {
            // Choose an unused port at random
            _oscSender = new OscSender(IPAddress.Loopback, 0,  9000);
            _oscSender.Connect();
            
            _device = device;

            if (_device != null)
            {
                _device.PropertyChanged += OnDeviceChanged;
            }
        }

        ~DeviceHeartrateOscOutput()
        {
            _device.PropertyChanged -= OnDeviceChanged;
            _oscSender?.Dispose();
            Cancel();
        }

        private void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private void OnDeviceChanged(object sender, PropertyChangedEventArgs e)
        {
            var device = sender as Device;
            if(device == null) return;
            
            switch (e.PropertyName)
            {
                case "HeartrateMonitorStarted":
                    if (device.HeartrateMonitorStarted)
                    {
                        OnHeartrateMonitorStarted();
                    }
                    else
                    {
                        OnHeartrateMonitorStopped();
                    }
                    break;
                case "Heartrate":
                    OnChangeHeartrate();
                    break;
            }
        }

        private void OnChangeHeartrate()
        {
            _oscSender.Send(new OscMessage(Addresses.HeartRateInt, (int)_device.Heartrate));
            _oscSender.Send(new OscMessage(Addresses.HeartRateFloat, _device.Heartrate / 255f));
        }

        private void OnHeartrateMonitorStarted()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _ = SendLoop(_cancellationTokenSource.Token);
        }

        private void OnHeartrateMonitorStopped()
        {
            Cancel();
        }

        private async Task SendLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_device.Heartrate == 0)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }
                var span = (int)(60.0f / _device.Heartrate * 1000);
                await Task.Delay(span, cancellationToken);
                var _ = Send(span, cancellationToken);
                
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private async Task Send(int span, CancellationToken cancellationToken)
        {
            _oscSender.Send(new OscMessage(Addresses.HeartBeatInt, 1));
            _oscSender.Send(new OscMessage(Addresses.HeartBeatPulse, true));
            _oscSender.Send(new OscMessage(Addresses.HeartBeatToggle, _currentBeatToggle));
            
            // Wait for QRS interval
            await Task.Delay(span / 5, cancellationToken);
            
            _oscSender.Send(new OscMessage(Addresses.HeartBeatInt, 0));
            _oscSender.Send(new OscMessage(Addresses.HeartBeatPulse, false));
            
            _currentBeatToggle = !_currentBeatToggle;
        }
    }
}