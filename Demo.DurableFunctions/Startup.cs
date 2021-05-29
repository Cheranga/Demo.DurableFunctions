using Demo.DurableFunctions;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.DTO.Responses;
using Demo.DurableFunctions.ResponseFormatters;
using Demo.DurableFunctions.Services;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Demo.DurableFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            RegisterServices(services);
            RegisterValidators(services);
            RegisterMappers(services);
        }

        private void RegisterMappers(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(Startup).Assembly
            };

            services.AddAutoMapper(assemblies, ServiceLifetime.Singleton);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IResponseFormatter<RegisterAccountResponse>, RegisterAccountResponseFormatter>();
            services.AddSingleton<IRegisterBankAccountService, RegisterBankAccountService>();
            services.AddSingleton<IHttpRequestBodyReader, HttpRequestBodyReader>();

            services.AddSingleton<ICheckVisaService, CheckVisaService>();
            services.AddSingleton<ICheckDriverLicenseService, CheckDriverLicenseService>();
        }

        private void RegisterValidators(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(Startup).Assembly
            };

            services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Singleton);
        }
    }
}