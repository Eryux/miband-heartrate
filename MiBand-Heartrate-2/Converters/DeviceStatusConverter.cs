using System;
using System.Globalization;
using System.Windows.Data;

namespace MiBand_Heartrate_2
{
    public class DeviceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string r = "No device connected";

            if (value is Devices.Device)
            {
                Devices.Device device = (Devices.Device)value;

                switch (device.Status)
                {
                    case Devices.DeviceStatus.ONLINE_UNAUTH:
                        r = string.Format("Connected to {0} | Not auth", device.Name);
                        break;
                    case Devices.DeviceStatus.ONLINE_AUTH:
                        r = string.Format("Connected to {0} | Auth", device.Name);
                        break;
                }
            }

            return r;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
