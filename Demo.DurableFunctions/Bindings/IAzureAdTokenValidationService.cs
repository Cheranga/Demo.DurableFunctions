using System.Security.Claims;
using System.Threading.Tasks;

namespace Demo.DurableFunctions.Bindings
{
    public interface IAzureAdTokenValidationService
    {
        Task<ClaimsPrincipal> GetClaimsPrincipalAsync(AzureAdTokenAttribute azureAdData);
    }
}