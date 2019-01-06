using System;
using System.Windows.Forms;

namespace MiBand_Heartrate
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            DeviceControl.Instance.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConnectionFrame());
        }
    }
}
