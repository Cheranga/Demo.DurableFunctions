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

            services.AddAutoMapper(assemblies);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IResponseFormatter<RegisterAccountResponse>, RegisterAccountResponseFormatter>();
            services.AddScoped<IRegisterBankAccountService, RegisterBankAccountService>();
            services.AddSingleton<IHttpRequestBodyReader, HttpRequestBodyReader>();
        }

        private void RegisterValidators(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(Startup).Assembly
            };

            services.AddValidatorsFromAssemblies(assemblies);
        }
    }
}