using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaRentalApp.Models;
using AvaloniaRentalApp.Services;

namespace AvaloniaRentalApp.Tests.Services;

[TestFixture]
public class ReportServiceTests
{
    private const int Year = 2026;
    private const int Month = 6; // June, 30 days

    private ReportService _reports = null!;
    private List<Car> _cars = null!;
    private List<Rental> _rentals = null!;
    private List<MaintenanceRecord> _maintenance = null!;
    private List<Customer> _customers = null!;
    private List<FuelPrice> _fuelPrices = null!;

    [SetUp]
    public void SetUp()
    {
        _reports = new ReportService();

        _cars = new List<Car>
        {
            new() { CarId = 1, Brand = "Toyota", Model = "Yaris", Registration = "REG1",
                    CategoryName = "Ekonomiczny", IsActive = true },
            new() { CarId = 2, Brand = "Volvo", Model = "XC60", Registration = "REG2",
                    CategoryName = "SUV", IsActive = true },
        };

        _rentals = new List<Rental>
        {
            // Paid, returned in June (5 days, fuel 200 km)
            new()
            {
                RentalNumber = "WYP-1", CustomerName = "Jan Kowalski", CarId = 1,
                DateStart = new DateTime(Year, 6, 1), DateEndActual = new DateTime(Year, 6, 5),
                DateEndPlanned = new DateTime(Year, 6, 5),
                Status = "zakonczona", PaymentStatus = "oplacona", PaymentMethod = "gotowka",
                TotalCost = 1000m, EmployeeName = "Anna",
                CarFuelType = "benzyna", CarFuelConsumption = 7.5m, MileageStart = 0, MileageEnd = 200,
            },
            // Active, arrears, not returned (3 days from planned end)
            new()
            {
                RentalNumber = "WYP-2", CustomerName = "Ewa Nowak", CarId = 2,
                DateStart = new DateTime(Year, 6, 10), DateEndPlanned = new DateTime(Year, 6, 12),
                Status = "aktywna", PaymentStatus = "zalegla", TotalCost = 500m,
            },
            // Previous-month paid rental (for trend / out-of-month exclusion)
            new()
            {
                RentalNumber = "WYP-3", CustomerName = "Adam Wiśniewski", CarId = 1,
                DateStart = new DateTime(Year, 5, 20), DateEndActual = new DateTime(Year, 5, 25),
                DateEndPlanned = new DateTime(Year, 5, 25),
                Status = "zakonczona", PaymentStatus = "oplacona", TotalCost = 300m,
            },
            // Cancelled in June — must be excluded from category/arrears/utilization
            new()
            {
                RentalNumber = "WYP-4", CustomerName = "Piotr Zieliński", CarId = 2,
                DateStart = new DateTime(Year, 6, 15), DateEndPlanned = new DateTime(Year, 6, 18),
                Status = "anulowana", PaymentStatus = "oczekuje", TotalCost = 999m,
            },
        };

        _maintenance = new List<MaintenanceRecord>
        {
            new() { CarId = 1, Date = new DateTime(Year, 6, 3), Cost = 150m },   // in June
            new() { CarId = 2, Date = new DateTime(Year, 5, 1), Cost = 999m },   // out of month
        };

        _customers = new List<Customer>
        {
            new() { CustomerId = 1, FirstName = "Jan", LastName = "Kowalski",
                    IsActive = true, CreatedAt = new DateTime(Year, 6, 2) },     // new in June
            new() { CustomerId = 2, FirstName = "Ewa", LastName = "Nowak",
                    IsActive = true, CreatedAt = new DateTime(Year, 1, 1) },     // existing
        };

        _fuelPrices = new List<FuelPrice>
        {
            new() { FuelType = "benzyna", PricePerUnit = 6.50m, Unit = "l" },
        };
    }

    // ===== Monthly summary ===================================================

    [Test]
    public void MonthlySummary_CountsRentalsReturnsAndNewCustomersInMonth()
    {
        var r = _reports.MonthlySummary(_rentals, _maintenance, _fuelPrices, _customers, _cars, Year, Month);

        Assert.That(r.RentalCount, Is.EqualTo(3));      // WYP-1, WYP-2, WYP-4 start in June
        Assert.That(r.CompletedReturns, Is.EqualTo(1)); // WYP-1 returned in June
        Assert.That(r.NewCustomers, Is.EqualTo(1));     // customer 1
    }

    [Test]
    public void MonthlySummary_RevenueIsGrossMinusFuelMinusMaintenance()
    {
        var r = _reports.MonthlySummary(_rentals, _maintenance, _fuelPrices, _customers, _cars, Year, Month);

        Assert.That(r.Revenue.Gross, Is.EqualTo(1000m));
        Assert.That(r.Revenue.Fuel, Is.EqualTo(97.50m));   // (200/100)*7.5*6.50
        Assert.That(r.Revenue.Maintenance, Is.EqualTo(150m));
        Assert.That(r.Revenue.Net, Is.EqualTo(752.50m));
    }

    [Test]
    public void MonthlySummary_ByCategoryExcludesCancelledAndSumsCost()
    {
        var r = _reports.MonthlySummary(_rentals, _maintenance, _fuelPrices, _customers, _cars, Year, Month);

        // WYP-4 (anulowana) excluded → Ekonomiczny 1000, SUV 500
        Assert.That(r.ByCategory.Count, Is.EqualTo(2));
        var eco = r.ByCategory.Single(c => c.Label == "Ekonomiczny");
        Assert.That(eco.Amount, Is.EqualTo(1000m));
        Assert.That(eco.Count, Is.EqualTo(1));
    }

    // ===== Fleet utilization =================================================

    [Test]
    public void FleetUtilization_EstimatesRentedAndIdleDaysClippedToMonth()
    {
        var r = _reports.FleetUtilization(_cars, _rentals, _maintenance, Year, Month);

        Assert.That(r.PeriodDays, Is.EqualTo(30));

        var car1 = r.Cars.Single(c => c.Registration == "REG1");
        Assert.That(car1.RentedDays, Is.EqualTo(5));   // June 1–5 inclusive
        Assert.That(car1.ServiceDays, Is.EqualTo(1));  // one maintenance record in June
        Assert.That(car1.IdleDays, Is.EqualTo(24));    // 30 − 5 − 1
        Assert.That(car1.KmDriven, Is.EqualTo(200));
        Assert.That(car1.Revenue, Is.EqualTo(1000m));

        var car2 = r.Cars.Single(c => c.Registration == "REG2");
        Assert.That(car2.RentedDays, Is.EqualTo(3));   // June 10–12 inclusive
        Assert.That(car2.IdleDays, Is.EqualTo(27));
    }

    [Test]
    public void FleetUtilization_PopularCategoriesIgnoresCancelledRentals()
    {
        var r = _reports.FleetUtilization(_cars, _rentals, _maintenance, Year, Month);

        // WYP-4 (anulowana) excluded → one rental per category
        Assert.That(r.PopularCategories.Count, Is.EqualTo(2));
        Assert.That(r.PopularCategories.Sum(p => p.RentalCount), Is.EqualTo(2));
    }

    // ===== Revenue report ====================================================

    [Test]
    public void RevenueReport_GroupsPaidIncomeByPaymentMethod()
    {
        var r = _reports.RevenueReport(_rentals, _maintenance, _fuelPrices, _cars, Year, Month);

        Assert.That(r.ByMethod.Count, Is.EqualTo(1));
        var cash = r.ByMethod.Single();
        Assert.That(cash.Method, Is.EqualTo("Gotówka"));
        Assert.That(cash.Amount, Is.EqualTo(1000m));
        Assert.That(cash.Count, Is.EqualTo(1));
    }

    [Test]
    public void RevenueReport_ArrearsListOutstandingRentalsAndTotal()
    {
        var r = _reports.RevenueReport(_rentals, _maintenance, _fuelPrices, _cars, Year, Month);

        // Only WYP-2 (zalegla); WYP-4 cancelled is excluded, WYP-1 is paid.
        Assert.That(r.Arrears.Count, Is.EqualTo(1));
        Assert.That(r.Arrears.Single().RentalNumber, Is.EqualTo("WYP-2"));
        Assert.That(r.ArrearsTotal, Is.EqualTo(500m));
    }

    [Test]
    public void RevenueReport_ByEmployeeSumsPaidIncome()
    {
        var r = _reports.RevenueReport(_rentals, _maintenance, _fuelPrices, _cars, Year, Month);

        var anna = r.ByEmployee.Single(e => e.Label == "Anna");
        Assert.That(anna.Amount, Is.EqualTo(1000m));
    }

    [Test]
    public void RevenueReport_TrendCoversSixMonthsEndingAtSelectedMonth()
    {
        var r = _reports.RevenueReport(_rentals, _maintenance, _fuelPrices, _cars, Year, Month);

        Assert.That(r.Trend.Count, Is.EqualTo(6));

        var june = r.Trend.Last();
        Assert.That(june.Month, Is.EqualTo(6));
        Assert.That(june.Gross, Is.EqualTo(1000m));
        Assert.That(june.BarPercent, Is.EqualTo(100.0)); // highest month

        var may = r.Trend.Single(p => p.Month == 5);
        Assert.That(may.Gross, Is.EqualTo(300m));        // WYP-3 paid in May
        Assert.That(may.BarPercent, Is.EqualTo(30.0));   // 300 / 1000
    }
}
