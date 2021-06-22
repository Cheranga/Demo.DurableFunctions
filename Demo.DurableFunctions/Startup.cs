using Demo.DurableFunctions;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.Infrastructure.DataAccess;
using Demo.DurableFunctions.ResponseFormatters;
using Demo.DurableFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Bootstrapper = Demo.DurableFunctions.Infrastructure.DataAccess.Bootstrapper;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Demo.DurableFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var configuration = GetConfigurationRoot(builder);
            
            RegisterServices(services);
            RegisterMappers(services);
            
            services.RegisterCoreServices()
                .RegisterDataAccess(configuration);
        }

        private void RegisterMappers(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(Startup).Assembly,
                typeof(Bootstrapper).Assembly
            };

            services.AddAutoMapper(assemblies, ServiceLifetime.Singleton);
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IResponseFormatter<RegisterAccountResponse>, RegisterAccountResponseFormatter>();
            services.AddSingleton<IRegisterBankAccountService, RegisterBankAccountService>();
            services.AddSingleton<IHttpRequestBodyReader, HttpRequestBodyReader>();

            return services;
        }
        
        protected virtual IConfigurationRoot GetConfigurationRoot(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var executionContextOptions = services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}