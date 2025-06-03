using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string StartLocation { get; set; }

        [Required]
        public string EndLocation { get; set; }

        [Required]
        public DateTime TripDateTime { get; set; }

        [Range(1, 10)]
        public int Passengers { get; set; }

        [Required]
        [RegularExpression("Economic|Premium|Executive")]
        public string CabType { get; set; }

        public bool IsPast => TripDateTime < DateTime.Now;
    }
}
