using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.DurableFunctions.Core.Domain
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterCoreDomain(this IServiceCollection services)
        {
            RegisterValidators(services);
            return services;
        }
        
        private static void RegisterValidators(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(Bootstrapper).Assembly
            };

            services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Singleton);
        }
    }
}