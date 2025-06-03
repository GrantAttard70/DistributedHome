using System.Threading.Tasks;

namespace PaymentService.Services
{
    public class CabFareService : ICabFareService
    {
        // Simulates getting base fare from an external API
        public async Task<decimal> GetBaseFareAsync()
        {
            await Task.Delay(100); // simulate latency
            return 10m; // fixed base fare for example
        }
    }
}
