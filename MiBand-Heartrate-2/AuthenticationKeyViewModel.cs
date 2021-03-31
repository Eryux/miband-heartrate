using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace MiBand_Heartrate_2
{
    public class AuthenticationKeyViewModel : ViewModel
    {

        string _key = "";

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                InvokePropertyChanged("Key");
            }
        }

        public AuthenticationKeyWindow View { get; set; } = null;

        // --------------------------------------

        ICommand _command_valid;

        public ICommand Command_Valid
        {
            get
            {
                if (_command_valid == null)
                {
                    _command_valid = new RelayCommand<object>("auth.valid", "Validate authentication key", o =>
                    {
                        if (!Regex.IsMatch(Key, @"^[0-9a-f]{32}$", RegexOptions.IgnoreCase))
                        {
                            Extras.MessageWindow.ShowError("Authentication key is not valid.", MessageBoxButton.OK);
                            return;
                        }

                        View.AuthenticationKeyResult = Key;
                        View.DialogResult = true;
                        View.Close();
                    });
                }

                return _command_valid;
            }
        }

        ICommand _command_cancel;

        public ICommand Command_Cancel
        {
            get
            {
                if (_command_cancel == null)
                {
                    _command_cancel = new RelayCommand<object>("auth.cancel", "Cancel authentication", o =>
                    {
                        View.DialogResult = false;
                        View.Close();
                    });
                }

                return _command_cancel;
            }
        }
    }
}
