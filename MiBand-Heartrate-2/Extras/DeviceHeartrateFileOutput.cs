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
                    using (var f = File.Open(_filename, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(_device.Heartrate.ToString());
                        f.Write(data, 0, data.Length);
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