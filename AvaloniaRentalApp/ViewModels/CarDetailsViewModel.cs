using AvaloniaRentalApp.Models;
using ReactiveUI;
using System.Reactive;

namespace AvaloniaRentalApp.ViewModels;

public class CarDetailsViewModel : ViewModelBase
{
    public Car Car { get; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public CarDetailsViewModel(Car car)
    {
        Car = car;
        CloseCommand = ReactiveCommand.Create(() => {});
    }
}
