using Demo.DurableFunctions.Core.Application.Services;
using Demo.DurableFunctions.Core.Domain.Services;
using Demo.DurableFunctions.Core.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.DurableFunctions.Core.Application
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterCoreApplication(this IServiceCollection services)
        {
            RegisterServices(services);

            return services;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ICheckVisaService, CheckVisaService>();
            services.AddSingleton<ICheckDriverLicenseService, CheckDriverLicenseService>();
            services.AddSingleton<IVerifyDocumentsService, VerifyDocumentsService>();
        }
    }
}