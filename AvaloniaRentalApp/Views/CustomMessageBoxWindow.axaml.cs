using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaRentalApp.Views
{
    public partial class CustomMessageBoxWindow : Window
    {
        public CustomMessageBoxWindow()
        {
            InitializeComponent();
        }

        public CustomMessageBoxWindow(string title, string message, bool isError = false) : this()
        {
            TitleBlock.Text = title;
            MessageBlock.Text = message;
            
            if (isError)
            {
                TopBar.Background = Avalonia.Application.Current?.FindResource("Red") as Avalonia.Media.IBrush;
                OkButton.Background = Avalonia.Application.Current?.FindResource("Red") as Avalonia.Media.IBrush;
            }
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
