using Demo.DurableFunctions.Core.Application.DataAccess;
using Demo.DurableFunctions.Infrastructure.DataAccess.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Demo.DurableFunctions.Infrastructure.DataAccess
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                return services;
            }

            services.AddSingleton<ITableStorageFactory, StorageTableFactory>();
            services.AddSingleton<ICommandHandler<CreateCustomerCommand>, CreateCustomerCommandHandler>();
            services.AddSingleton<ICommandHandler<UpdateCustomerCommand>, UpdateCustomerCommandHandler>();
            services.AddSingleton<ICommandHandler<CreateBankAccountCommand>, CreateBankAccountCommandHandler>();
            
            services.Configure<DatabaseConfig>(configuration.GetSection(nameof(DatabaseConfig)));
            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<DatabaseConfig>>().Value;
                return config;
            });

            return services;
        }
    }
}