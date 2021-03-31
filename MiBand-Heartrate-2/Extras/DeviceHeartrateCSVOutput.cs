using System;
using System.IO;
using System.ComponentModel;

using MiBand_Heartrate_2.Devices;

namespace MiBand_Heartrate_2.Extras
{
    public class DeviceHeartrateCSVOutput
    {
        Device _device;

        string _filename;

        public DeviceHeartrateCSVOutput(string filename, Device device)
        {
            _filename = filename;

            _device = device;

            if (_device != null)
            {
                _device.PropertyChanged += OnDeviceChanged;
            }
        }

        ~DeviceHeartrateCSVOutput()
        {
            _device.PropertyChanged -= OnDeviceChanged;
        }

        private void OnDeviceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Heartrate")
            {
                try
                {
                    if (!File.Exists(_filename))
                    {
                        using (StreamWriter f = new StreamWriter(_filename))
                        {
                            f.WriteLine("At,Heartrate");
                        }
                    }

                    using (StreamWriter f = new StreamWriter(_filename, true))
                    {
                        f.WriteLine(string.Format("{0},{1}", DateTime.Now, _device.Heartrate));
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