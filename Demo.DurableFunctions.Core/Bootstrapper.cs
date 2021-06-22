using Demo.DurableFunctions.Core.Application.Services;
using Demo.DurableFunctions.Core.Domain.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.DurableFunctions.Core
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services)
        {
            RegisterValidators(services);
            RegisterServices(services);

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

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ICheckVisaService, CheckVisaService>();
            services.AddSingleton<ICheckDriverLicenseService, CheckDriverLicenseService>();
            services.AddSingleton<IVerifyDocumentsService, VerifyDocumentsService>();
        }
    }
}