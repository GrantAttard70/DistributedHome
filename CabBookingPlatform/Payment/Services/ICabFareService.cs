using System.Threading.Tasks;

namespace PaymentService.Services
{
    public interface ICabFareService
    {
        Task<decimal> GetBaseFareAsync();
    }
}
