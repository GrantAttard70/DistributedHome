using Microsoft.EntityFrameworkCore;
using LocationService.Models;
using System.Collections.Generic;

namespace LocationService.Data
{
    public class LocationDbContext : DbContext
    {
        public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options) { }

        public DbSet<FavouriteLocation> FavouriteLocations { get; set; }
    }
}

