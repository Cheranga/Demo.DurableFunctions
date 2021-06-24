using System.Security.Claims;
using System.Threading.Tasks;

namespace Demo.DurableFunctions.Bindings
{
    public class DummyTokenValidationService : IAzureAdTokenValidationService
    {
        public Task<ClaimsPrincipal> GetClaimsPrincipalAsync(AzureAdTokenAttribute azureAdData)
        {
            var principal = new ClaimsPrincipal();
            return Task.FromResult(principal);
        }
    }
}