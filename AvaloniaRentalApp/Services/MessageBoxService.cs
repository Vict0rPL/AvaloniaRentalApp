using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using AvaloniaRentalApp.Views;

namespace AvaloniaRentalApp.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public async Task ShowErrorMessageAsync(string title, string message)
        {
            await ShowBox(title, message, true);
        }

        public async Task ShowInfoMessageAsync(string title, string message)
        {
            await ShowBox(title, message, false);
        }

        private async Task ShowBox(string title, string message, bool isError)
        {
            var window = new CustomMessageBoxWindow(title, message, isError);

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (desktop.MainWindow != null)
                {
                    await window.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    window.Show();
                }
            }
            else
            {
                window.Show();
            }
        }
    }
}
