namespace ApiGateway.Models
{
    public record RegisterCustomerDto(string FirstName,
                                      string Surname,
                                      string Email,
                                      string Password);
}
