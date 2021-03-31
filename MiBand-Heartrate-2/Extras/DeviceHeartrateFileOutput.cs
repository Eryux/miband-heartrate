using System;
using System.IO;
using System.ComponentModel;

using MiBand_Heartrate_2.Devices;

namespace MiBand_Heartrate_2.Extras
{
    public class DeviceHeartrateFileOutput
    {
        Device _device;

        string _filename;

        public DeviceHeartrateFileOutput(string filename, Device device)
        {
            _filename = filename;

            _device = device;

            if (_device != null)
            {
                _device.PropertyChanged += OnDeviceChanged;
            }
        }

        ~DeviceHeartrateFileOutput()
        {
            _device.PropertyChanged -= OnDeviceChanged;
        }

        private void OnDeviceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Heartrate")
            {
                try
                {
                    using (StreamWriter f = new StreamWriter(_filename))
                    {
                        f.Write(_device.Heartrate);
                    }
                }
                catch (Exception err)
                {
                    MessageWindow.ShowError(err.ToString());
                }
            }
        }
    }
}