using System.Data.Common;
using DelegateRetry.Core;
using DelegateRetry.Db;
using DelegateRetry.Models;
using JobUtils.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DelegateRetry.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRetryServices(this IServiceCollection services, IConfiguration configuration, Func<string, DbConnection> connectionFactory)
        {
            // Add RetryOptions
            services.Configure<RetryOptions>(configuration.GetSection("RetryOptions"));
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<RetryOptions>>().Value);
            
            // Add TaskStorage
            services.AddSingleton<TaskStorage>(provider =>
            {
                var retryOptions = provider.GetRequiredService<RetryOptions>();
                var logger = provider.GetRequiredService<ILogger<TaskStorage>>();
                return new TaskStorage(retryOptions, () => connectionFactory(retryOptions.StorageOptions.ConnectionString), logger);
            });
            
            
            // Add Services to the container.
            // Add DelegateService
            services.AddSingleton<IDelegateService, DelegateService>();

            //collect registered methods before worker started, because worker will try to get the impl of registered methods
            MethodRegistry.CollectRegisteredMethods(); 
            // Add RetryHandler
            services.AddSingleton<IHandler>(provider =>
            {
                var retryOptions = provider.GetRequiredService<RetryOptions>();
                var logger = provider.GetRequiredService<ILogger<RetryHandler>>();
                var storage = provider.GetRequiredService<TaskStorage>();
                return new RetryHandler(retryOptions, logger, storage);
            });
            
            var tempServiceProvider = services.BuildServiceProvider();
            var handler= tempServiceProvider.GetRequiredService<IHandler>();
            var retryOptions= tempServiceProvider.GetRequiredService<RetryOptions>();

            // Configure WorkerService with RetryHandler
            services.AddWorkerService(retryOptions.WorkerOptions, handler);
            // Add RetryHandler

            return services;
        }
    }
}