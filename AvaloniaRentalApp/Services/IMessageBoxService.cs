using System.Threading.Tasks;

namespace AvaloniaRentalApp.Services
{
    public interface IMessageBoxService
    {
        Task ShowErrorMessageAsync(string title, string message);
        Task ShowInfoMessageAsync(string title, string message);
    }
}
