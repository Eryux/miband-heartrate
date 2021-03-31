using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiBand_Heartrate_2
{
    /// <summary>
    /// Logique d'interaction pour ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        ConnectionWindowViewModel _model;

        public ConnectionWindow(MainWindowViewModel main)
        {
            InitializeComponent();

            _model = (ConnectionWindowViewModel)DataContext;

            if (_model != null)
            {
                _model.View = this;
                _model.Main = main;
            }
        }
    }
}
