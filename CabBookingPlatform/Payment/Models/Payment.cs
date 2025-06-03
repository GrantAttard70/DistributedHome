using System;

namespace PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentStatus { get; set; } = "Paid";
        public int Passengers { get; set; }
        public CabType CabType { get; set; }
        public DateTime TripDateTime { get; set; }
        public decimal Discount { get; set; } = 1m;
    }
}