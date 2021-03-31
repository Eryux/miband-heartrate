using System.Windows;

namespace MiBand_Heartrate_2.Extras
{
    public static class MessageWindow
    {
        public static string Title = "Heartrate Monitor";

        public static MessageBoxResult Show(string message, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return MessageBox.Show(message, Title, buttons, MessageBoxImage.None);
        }

        public static MessageBoxResult ShowError(string message, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return MessageBox.Show(message, Title + " - Error", buttons, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowWarning(string message, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            return MessageBox.Show(message, Title + " - Warning", buttons, MessageBoxImage.Warning);
        }

    }
}
