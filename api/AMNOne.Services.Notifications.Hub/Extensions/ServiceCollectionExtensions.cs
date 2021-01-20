using AMNOne.Framework.ExceptionHandling.Middleware.Behaviour;
using AMNOne.Services.Notifications.Application.Configuration;
using AMNOne.Services.Notifications.Application.Mapping;
using AMNOne.Services.Notifications.Application.Notifications.Handlers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace AMNOne.Services.Notifications.Hub.Extensions
{
    /// <summary>
    /// ServiceCollection Extension Pattern
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddOptionConfiguration(this IServiceCollection services)
        {
            services.AddOptions<ApplicationOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(ApplicationOptions.Application).Bind(settings);
                });

            services.AddOptions<ServiceBusOptions>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.GetSection(ServiceBusOptions.Name).Bind(settings);
                   settings.ConnectionString = configuration["ServiceBus:ConnectionString"];
               });


            services.AddOptions<SenGridOptions>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.GetSection(SenGridOptions.Name).Bind(settings);
                   //TODO : Add this key to key vault secret names
                   settings.ApiKey = configuration["SendGrid-API-AMNOne-Notification-Dev"];
               });
        }

        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ApplicationProfile).Assembly);
            services.AddValidatorsFromAssembly(typeof(ApplicationProfile).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }

        public static void AddHealthChecksWithDependencies(this IServiceCollection services,
                                                         string cosmosConnectionString,
                                                         string databaseName)
        {
            services.AddHealthChecks()
                .AddCosmosDb(cosmosConnectionString,
                                databaseName,
                                name: "CosmosDB",
                                failureStatus: HealthStatus.Unhealthy,
                                timeout: new TimeSpan(0, 0, 30),
                                tags: new string[] { "readiness" })
                ;
        }
    }
}
