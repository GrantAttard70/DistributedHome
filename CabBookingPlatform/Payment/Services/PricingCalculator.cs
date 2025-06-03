using System;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class PricingCalculator : IPricingCalculator
    {
        public decimal CalculateTotalPrice(
            decimal cabFare,
            CabType cabType,
            DateTime tripDateTime,
            int passengers,
            decimal discountMultiplier = 1m)
        {
            // Validate passengers
            if (passengers < 1 || passengers > 8)
                throw new ArgumentException("Passengers must be between 1 and 8.");

            // Validate discount
            if (discountMultiplier < 0 || discountMultiplier > 1)
                throw new ArgumentException("Discount multiplier must be between 0 and 1.");

            // Cab type multiplier
            decimal cabMultiplier = cabType switch
            {
                CabType.Economic => 1m,
                CabType.Premium => 1.2m,
                CabType.Executive => 1.4m,
                _ => throw new ArgumentException("Invalid cab type.")
            };

            // Daytime multiplier: 12 AM - 7:59 AM = 1.2, else 1
            decimal daytimeMultiplier = (tripDateTime.Hour >= 8 && tripDateTime.Hour <= 23) ? 1m : 1.2m;

            // Passenger multiplier
            decimal passengerMultiplier = passengers switch
            {
                >= 1 and <= 4 => 1m,
                >= 5 and <= 8 => 2m,
                _ => throw new ArgumentException("Passengers > 8 not allowed.")
            };

            // Calculate total price
            var totalPrice = cabFare * cabMultiplier * daytimeMultiplier * passengerMultiplier * discountMultiplier;

            return Math.Round(totalPrice, 2);
        }
    }
}
