using System;

namespace PaymentService.Models
{
    public class PaymentAudit
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}