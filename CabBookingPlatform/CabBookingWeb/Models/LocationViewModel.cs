using System.ComponentModel.DataAnnotations;

public class LocationViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Label { get; set; }

    [Required]
    public string Address { get; set; }
}
