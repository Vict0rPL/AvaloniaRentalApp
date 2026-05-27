using System;
using AvaloniaRentalApp.Models;

namespace AvaloniaRentalApp.Services
{
    public static class RentalCalculator
    {
        public const decimal LATE_PENALTY_MULTIPLIER = 1.5m;

        public static decimal CalculateLateReturnCost(DateTime plannedEnd, DateTime actualEnd, decimal dailyRate)
        {
            if (actualEnd.Date <= plannedEnd.Date)
                return 0;

            int lateDays = (actualEnd.Date - plannedEnd.Date).Days;
            return lateDays * dailyRate * LATE_PENALTY_MULTIPLIER;
        }

        public static decimal CalculateExtraKmCost(int mileageStart, int mileageEnd, int totalDays, int? mileageLimit, decimal? extraKmRate)
        {
            if (mileageLimit == null || extraKmRate == null)
                return 0;
            if (mileageEnd <= mileageStart)
                return 0;

            int drivenKm = mileageEnd - mileageStart;
            int allowedKm = mileageLimit.Value * totalDays;
            int extraKm = drivenKm - allowedKm;

            if (extraKm <= 0)
                return 0;

            return Math.Round(extraKm * extraKmRate.Value, 2);
        }

        public static decimal CalculateTotalCost(Rental rental)
        {
            decimal total = rental.BaseCost + rental.ExtraKmCost + rental.LateReturnCost + rental.DamageCost;
            
            if (rental.DiscountPercent > 0)
            {
                total -= total * (rental.DiscountPercent / 100);
            }
            
            return Math.Round(total, 2);
        }
    }
}
