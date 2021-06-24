using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Demo.DurableFunctions.Bindings
{
    public static class AzureAdTokenBindingExtension
    {
        public static IWebJobsBuilder UseAzureAdTokenBinding(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            
            builder.AddExtension<AzureAdTokenBinding>();
            
            builder.Services.AddSingleton<ISecurityTokenValidator, JwtSecurityTokenHandler>();

            if (IsLocal())
            {
                builder.Services.AddSingleton<IAzureAdTokenValidationService, DummyTokenValidationService>();
            }
            else
            {
                builder.Services.AddSingleton<IAzureAdTokenValidationService, AzureAdTokenValidationService>();
            }
            
            return builder;
        }

        private static bool IsLocal()
        {
            var storageAccount = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            return string.Equals(storageAccount, "UseDevelopmentStorage=true");
        }
    }
}