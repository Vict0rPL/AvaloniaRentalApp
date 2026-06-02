using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Services;

/// <summary>
/// Pure-calculation helpers behind the Reports view. 
/// </summary>
public class ReportService
{
    private readonly PricingService _pricing = new();

    /// <summary>Polish month names, indexed 1–12 (index 0 unused).</summary>
    public static readonly string[] MonthNames =
    {
        "", "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec",
        "Lipiec", "Sierpień", "Wrzesień", "Październik", "Listopad", "Grudzień"
    };

    public static string MonthLabel(int year, int month) => $"{MonthNames[month]} {year}";

    // ===== 1. Monthly report =================================================

    /// <summary>
    /// Monthly summary
    /// </summary>
    public MonthlySummaryResult MonthlySummary(
        IEnumerable<Rental> rentals,
        IEnumerable<MaintenanceRecord> maintenance,
        IEnumerable<FuelPrice> fuelPrices,
        IEnumerable<Customer> customers,
        IReadOnlyList<Car> cars,
        int year, int month)
    {
        var allRentals = rentals.ToList();
        var inMonth = allRentals.Where(r => InMonth(r.DateStart, year, month)).ToList();
        var prices = PricingService.BuildPriceMap(fuelPrices);

        var paid = inMonth.Where(r => r.PaymentStatus == "oplacona").ToList();
        var maintenanceInMonth = maintenance.Where(m => InMonth(m.Date, year, month)).ToList();
        var revenue = _pricing.Breakdown(paid, maintenanceInMonth, prices);

        var byStatus = inMonth
            .GroupBy(r => r.DisplayStatus)
            .Select(g => new CountAmountRow(g.Key, g.Count(), g.Sum(r => r.TotalCost)))
            .OrderByDescending(r => r.Count)
            .ToList();

        var categoryByCar = CategoryByCar(cars);
        var byCategory = inMonth
            .Where(r => r.Status != "anulowana")
            .GroupBy(r => CategoryName(categoryByCar, r.CarId))
            .Select(g => new CountAmountRow(g.Key, g.Count(), g.Sum(r => r.TotalCost)))
            .OrderByDescending(r => r.Amount)
            .ToList();

        return new MonthlySummaryResult(
            RentalCount: inMonth.Count,
            CompletedReturns: allRentals.Count(r => r.DateEndActual is { } d && InMonth(d, year, month)),
            NewCustomers: customers.Count(c => c.IsActive && InMonth(c.CreatedAt, year, month)),
            Revenue: revenue,
            ByStatus: byStatus,
            ByCategory: byCategory);
    }

    // ===== 2. Fleet utilization ==============================================

    /// <summary>
    /// Fleet utilization for the month.
    /// </summary>
    public FleetUtilizationResult FleetUtilization(
        IReadOnlyList<Car> cars,
        IEnumerable<Rental> rentals,
        IEnumerable<MaintenanceRecord> maintenance,
        int year, int month)
    {
        var periodDays = DateTime.DaysInMonth(year, month);
        var periodStart = new DateTime(year, month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        var rentalList = rentals.Where(r => r.Status != "anulowana").ToList();
        var maintenanceList = maintenance.ToList();

        var rows = new List<FleetUtilizationRow>();
        foreach (var car in cars.Where(c => c.IsActive))
        {
            var carRentals = rentalList.Where(r => r.CarId == car.CarId).ToList();

            var rentedDays = Math.Min(periodDays,
                carRentals.Sum(r => OverlapDays(r.DateStart, r.DateEndActual ?? r.DateEndPlanned,
                                                periodStart, periodEnd)));

            var serviceDays = Math.Min(periodDays - rentedDays,
                maintenanceList.Count(m => m.CarId == car.CarId && InMonth(m.Date, year, month)));

            var idleDays = Math.Max(0, periodDays - rentedDays - serviceDays);

            var startedInMonth = carRentals.Where(r => InMonth(r.DateStart, year, month)).ToList();

            rows.Add(new FleetUtilizationRow(
                CarName: car.FullName,
                Registration: car.Registration,
                CategoryName: car.CategoryName,
                RentedDays: rentedDays,
                ServiceDays: serviceDays,
                IdleDays: idleDays,
                PeriodDays: periodDays,
                KmDriven: startedInMonth.Sum(r => r.KmDriven),
                Revenue: startedInMonth.Sum(r => r.TotalCost)));
        }

        rows = rows.OrderByDescending(r => r.UtilizationPercent).ToList();
        var averageUtilization = rows.Count > 0
            ? Math.Round(rows.Average(r => r.UtilizationPercent), 1)
            : 0;

        var categoryByCar = CategoryByCar(cars);
        var grouped = rentalList
            .Where(r => InMonth(r.DateStart, year, month))
            .GroupBy(r => CategoryName(categoryByCar, r.CarId))
            .Select(g => (Category: g.Key, Count: g.Count()))
            .OrderByDescending(g => g.Count)
            .ToList();
        var maxCount = grouped.Count > 0 ? grouped.Max(g => g.Count) : 0;
        var popular = grouped
            .Select(g => new PopularCategoryRow(g.Category, g.Count,
                maxCount > 0 ? Math.Round(100.0 * g.Count / maxCount, 1) : 0))
            .ToList();

        return new FleetUtilizationResult(periodDays, averageUtilization, rows, popular);
    }

    // ===== 3. Revenue report =================================================

    /// <summary>
    /// Revenue report for the month
    /// </summary>
    public RevenueReportResult RevenueReport(
        IEnumerable<Rental> rentals,
        IEnumerable<MaintenanceRecord> maintenance,
        IEnumerable<FuelPrice> fuelPrices,
        IReadOnlyList<Car> cars,
        int year, int month)
    {
        var rentalList = rentals.ToList();
        var prices = PricingService.BuildPriceMap(fuelPrices);

        var paidInMonth = rentalList
            .Where(r => r.PaymentStatus == "oplacona" && InMonth(r.DateStart, year, month))
            .ToList();
        var maintenanceInMonth = maintenance.Where(m => InMonth(m.Date, year, month)).ToList();
        var revenue = _pricing.Breakdown(paidInMonth, maintenanceInMonth, prices);

        var byMethod = paidInMonth
            .GroupBy(r => PaymentMethodLabel(r.PaymentMethod))
            .Select(g => new PaymentMethodRow(g.Key, g.Count(), g.Sum(r => r.TotalCost)))
            .OrderByDescending(r => r.Amount)
            .ToList();

        var outstandingStatuses = new[] { "oczekuje", "czesciowa", "zalegla" };
        var arrears = rentalList
            .Where(r => r.Status != "anulowana"
                        && InMonth(r.DateStart, year, month)
                        && outstandingStatuses.Contains(r.PaymentStatus))
            .OrderByDescending(r => r.TotalCost)
            .Select(r => new ArrearsRow(r.RentalNumber, r.CustomerName, r.TotalCost,
                                        r.DisplayPayment, r.PaymentBadge))
            .ToList();
        var arrearsTotal = arrears.Sum(a => a.Amount);

        var categoryByCar = CategoryByCar(cars);
        var byCategory = paidInMonth
            .GroupBy(r => CategoryName(categoryByCar, r.CarId))
            .Select(g => new CountAmountRow(g.Key, g.Count(), g.Sum(r => r.TotalCost)))
            .OrderByDescending(r => r.Amount)
            .ToList();

        var byEmployee = paidInMonth
            .GroupBy(r => string.IsNullOrWhiteSpace(r.EmployeeName) ? "—" : r.EmployeeName)
            .Select(g => new CountAmountRow(g.Key, g.Count(), g.Sum(r => r.TotalCost)))
            .OrderByDescending(r => r.Amount)
            .ToList();

        var trend = RevenueTrend(rentalList, year, month, 6);

        return new RevenueReportResult(revenue, byMethod, arrears, arrearsTotal,
                                       byCategory, byEmployee, trend);
    }

    /// <summary>Gross paid income for each of the last <paramref name="months"/> months ending at (year, month).</summary>
    private static List<RevenueTrendPoint> RevenueTrend(
        IReadOnlyList<Rental> rentals, int year, int month, int months)
    {
        var raw = new List<(int Year, int Month, decimal Gross)>();
        var cursor = new DateTime(year, month, 1);
        for (var i = months - 1; i >= 0; i--)
        {
            var m = cursor.AddMonths(-i);
            var gross = rentals
                .Where(r => r.PaymentStatus == "oplacona" && InMonth(r.DateStart, m.Year, m.Month))
                .Sum(r => r.TotalCost);
            raw.Add((m.Year, m.Month, gross));
        }

        var max = raw.Count > 0 ? raw.Max(p => p.Gross) : 0m;
        return raw
            .Select(p => new RevenueTrendPoint(p.Year, p.Month, p.Gross,
                max > 0 ? Math.Round(100.0 * (double)(p.Gross / max), 1) : 0))
            .ToList();
    }

    // ---- helpers ------------------------------------------------------------

    private static bool InMonth(DateTime date, int year, int month)
        => date.Year == year && date.Month == month;

    /// <summary>Inclusive number of days the interval [start,end] overlaps [periodStart,periodEnd].</summary>
    private static int OverlapDays(DateTime start, DateTime end, DateTime periodStart, DateTime periodEnd)
    {
        var from = start.Date < periodStart ? periodStart : start.Date;
        var to = end.Date > periodEnd ? periodEnd : end.Date;
        var days = (to - from).Days + 1;
        return days > 0 ? days : 0;
    }

    private static Dictionary<int, string> CategoryByCar(IEnumerable<Car> cars)
        => cars.GroupBy(c => c.CarId).ToDictionary(g => g.Key, g => g.First().CategoryName);

    private static string CategoryName(IReadOnlyDictionary<int, string> map, int carId)
        => map.TryGetValue(carId, out var name) && !string.IsNullOrWhiteSpace(name) ? name : "—";

    private static string PaymentMethodLabel(string? method) => method switch
    {
        "gotowka" => "Gotówka",
        "karta"   => "Karta",
        "przelew" => "Przelew",
        _         => "Nieokreślona"
    };
}
