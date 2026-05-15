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
