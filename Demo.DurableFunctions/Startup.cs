using Demo.DurableFunctions;
using Demo.DurableFunctions.Bindings;
using Demo.DurableFunctions.Core;
using Demo.DurableFunctions.Core.Application;
using Demo.DurableFunctions.Core.Domain;
using Demo.DurableFunctions.Core.Domain.Responses;
using Demo.DurableFunctions.Infrastructure.DataAccess;
using Demo.DurableFunctions.ResponseFormatters;
using Demo.DurableFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Bootstrapper = Demo.DurableFunctions.Infrastructure.DataAccess.Bootstrapper;

// [assembly: FunctionsStartup(typeof(Startup))]
[assembly: WebJobsStartup(typeof(Startup))]

namespace Demo.DurableFunctions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.UseAzureAdTokenBinding();
            
            var services = builder.Services;

            var configuration = GetConfigurationRoot(builder);
            
            RegisterServices(services);
            RegisterMappers(services);

            services.RegisterCoreApplication()
                .RegisterCoreDomain()
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
        
        protected virtual IConfigurationRoot GetConfigurationRoot(IWebJobsBuilder builder)
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