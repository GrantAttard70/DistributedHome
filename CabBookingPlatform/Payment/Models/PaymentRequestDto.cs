using System;

namespace PaymentService.Models
{
    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public int Passengers { get; set; }
        public CabType CabType { get; set; }
        public DateTime TripDateTime { get; set; }
        public decimal Discount { get; set; } = 1m;
    }
}