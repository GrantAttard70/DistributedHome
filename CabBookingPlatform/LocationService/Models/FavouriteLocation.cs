namespace LocationService.Models
{
    public class FavouriteLocation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Link to user
        public string Name { get; set; } = string.Empty; // eg. "Home", "Work"
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
