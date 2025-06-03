using System;
using PaymentService.Models;

namespace PaymentService.Services
{
    public interface IPricingCalculator
    {
        decimal CalculateTotalPrice(
            decimal cabFare,
            CabType cabType,
            DateTime tripDateTime,
            int passengers,
            decimal discountMultiplier = 1m);
    }
}