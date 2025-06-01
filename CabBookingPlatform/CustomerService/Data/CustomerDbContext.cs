using CustomerService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CustomerService.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
