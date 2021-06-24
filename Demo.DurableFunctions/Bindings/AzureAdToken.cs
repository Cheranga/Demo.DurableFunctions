using System.Security.Claims;

namespace Demo.DurableFunctions.Bindings
{
    public class AzureAdToken
    {
        public ClaimsPrincipal User { get; set; }
    }
}