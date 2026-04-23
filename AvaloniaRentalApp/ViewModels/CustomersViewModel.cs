using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;
using ReactiveUI;

namespace AvaloniaRentalApp.ViewModels;

public class CustomersViewModel : ViewModelBase
{
    private readonly User _user;
    private readonly DatabaseService _dbService;
    private readonly List<Customer> _allCustomers = new();

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public ObservableCollection<Customer> Customers { get; } = new();

    public Interaction<AddCustomerViewModel, Customer?> ShowAddCustomerDialog { get; } = new();

    public ReactiveCommand<Unit, Unit> AddCustomerCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public CustomersViewModel(User user)
    {
        _user = user;
        _dbService = new DatabaseService();

        AddCustomerCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var addVm = new AddCustomerViewModel();
            var result = await ShowAddCustomerDialog.Handle(addVm);
            if (result is not null)
            {
                await _dbService.AddCustomerAsync(result);
                await LoadDataAsync();
            }
        });

        RefreshCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ApplyFilter());

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var customers = await _dbService.GetCustomersAsync();
        _allCustomers.Clear();
        _allCustomers.AddRange(customers);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = _allCustomers.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var t = SearchText;
            filtered = filtered.Where(c =>
                c.LastName.Contains(t, StringComparison.OrdinalIgnoreCase)  ||
                c.FirstName.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                c.Phone.Contains(t, StringComparison.OrdinalIgnoreCase)     ||
                (c.Email?.Contains(t, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Pesel?.Contains(t) ?? false));
        }
        Customers.Clear();
        foreach (var c in filtered) Customers.Add(c);
    }
}
