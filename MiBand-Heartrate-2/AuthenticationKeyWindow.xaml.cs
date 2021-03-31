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
    /// Logique d'interaction pour AuthenticationKeyWindow.xaml
    /// </summary>
    public partial class AuthenticationKeyWindow : Window
    {
        public string AuthenticationKeyResult { get; set; } = "";

        public AuthenticationKeyWindow()
        {
            InitializeComponent();

            AuthenticationKeyViewModel model = (AuthenticationKeyViewModel)DataContext;
            model.View = this;
        }
    }
}
