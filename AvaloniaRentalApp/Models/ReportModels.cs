using System.Collections.Generic;
using AvaloniaRentalApp.Services;

namespace AvaloniaRentalApp.Models;

// Result rows and aggregates produced by ReportService

/// <summary>A label with a count and a money amount (status / category / employee tables).</summary>
public record CountAmountRow(string Label, int Count, decimal Amount);

// ---- Monthly report ----

public record MonthlySummaryResult(
    int RentalCount,
    int CompletedReturns,
    int NewCustomers,
    PricingService.RevenueBreakdown Revenue,
    IReadOnlyList<CountAmountRow> ByStatus,
    IReadOnlyList<CountAmountRow> ByCategory);

// ---- Fleet utilization ----

public record FleetUtilizationRow(
    string CarName, string Registration, string CategoryName,
    int RentedDays, int ServiceDays, int IdleDays, int PeriodDays,
    int KmDriven, decimal Revenue)
{
    public double UtilizationPercent => PeriodDays > 0
        ? System.Math.Round(100.0 * RentedDays / PeriodDays, 1)
        : 0;
}

public record PopularCategoryRow(string CategoryName, int RentalCount, double BarPercent);

public record FleetUtilizationResult(
    int PeriodDays,
    double AverageUtilization,
    IReadOnlyList<FleetUtilizationRow> Cars,
    IReadOnlyList<PopularCategoryRow> PopularCategories);

// ---- Revenue report ----

public record PaymentMethodRow(string Method, int Count, decimal Amount);

public record ArrearsRow(string RentalNumber, string CustomerName, decimal Amount,
                         string DisplayPayment, string PaymentBadge);

public record RevenueTrendPoint(int Year, int Month, decimal Gross, double BarPercent)
{
    public string Label => $"{ReportService.MonthNames[Month]} {Year}";
}

public record RevenueReportResult(
    PricingService.RevenueBreakdown Revenue,
    IReadOnlyList<PaymentMethodRow> ByMethod,
    IReadOnlyList<ArrearsRow> Arrears,
    decimal ArrearsTotal,
    IReadOnlyList<CountAmountRow> ByCategory,
    IReadOnlyList<CountAmountRow> ByEmployee,
    IReadOnlyList<RevenueTrendPoint> Trend);
