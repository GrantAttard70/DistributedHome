using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public string StartLocation { get; set; } = string.Empty;

        [Required]
        public string EndLocation { get; set; } = string.Empty;

        [Required]
        public DateTime BookingTime { get; set; }

        [Required]
        public int Passengers { get; set; }

        [Required]
        public string CabType { get; set; } = "Economic"; // Economic, Premium, Executive

        [Required]
        public int CustomerId { get; set; } // from JWT
    }

}
