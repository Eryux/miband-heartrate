using System;
using System.Windows;

namespace MiBand_Heartrate_2
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel _model = null;

        public MainWindow()
        {
            InitializeComponent();
            _model = (MainWindowViewModel)DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_model != null)
            {
                _model.Command_Disconnect.Execute(null);
            }
        }
    }
}
