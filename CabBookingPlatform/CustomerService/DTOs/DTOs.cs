using CustomerService.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.DTOs
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class CustomerResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;


        public CustomerResponse(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            Surname = customer.Surname;
            Email = customer.Email;
        }


        public CustomerResponse() { }
    }


    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class DiscountEventRequest
    {
        public string UserId { get; set; } = null!;
    }

    public class BookingCompletedEvent
    {
        public string UserId { get; set; } = null!;
    }


}
