using System.ComponentModel.DataAnnotations;

public class BookingViewModel
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Pickup Location")]
    public string PickupLocation { get; set; }

    [Required]
    public string Destination { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan Time { get; set; }

    [Required]
    [Range(1, 8)]
    public int Passengers { get; set; }

    [Required]
    [Display(Name = "Cab Type")]
    public string CabType { get; set; }

    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }

    public string Status { get; set; }
}