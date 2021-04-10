using System;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

using MiBand_Heartrate_2.Extras;

namespace MiBand_Heartrate_2
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel _model = null;

        public MainWindow()
        {
            InitializeComponent();

            _model = (MainWindowViewModel)DataContext;

            // Restore window position
            Left = Setting.Get("WindowLeft", (int)Left);
            Top = Setting.Get("WindowTop", (int)Top);
            Width = Setting.Get("WindowWidth", (int)MinWidth);
            Height = Setting.Get("WindowHeight", (int)MinHeight);

            // Verify if window isn't out of screen
            if (!IsOnScreen())
            {
                CenterWindow();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_model != null)
            {
                _model.Command_Disconnect.Execute(null);
            }

            // Save window size and positions
            Setting.Set("WindowLeft", (int)Left);
            Setting.Set("WindowTop", (int)Top);
            Setting.Set("WindowWidth", (int)Width);
            Setting.Set("WindowHeight", (int)Height);
        }

        bool IsOnScreen()
        {
            var rect = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(rect));
        }

        void CenterWindow()
        {
            Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            Top = (SystemParameters.PrimaryScreenHeight / 2) - (Height / 2);
        }
    }
}
