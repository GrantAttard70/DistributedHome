using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }  

        [Required]
        public int CustomerId { get; set; }


        [Required]
        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
