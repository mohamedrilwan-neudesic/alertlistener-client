using System;
using System.Linq;
using AMNOne.Framework.AspNetCore;
using AMNOne.Framework.AspNetCore.Model;
using AMNOne.Framework.ExceptionHandling.Middleware.Behaviour;
using AMNOne.Framework.ExceptionHandling.Middleware.Builder;
using AMNOne.Framework.HealthChecks;
using AMNOne.Framework.Infrastructure;
using AMNOne.Framework.Utilities.Constants;
using AMNOne.Services.Notifications.Application.Interfaces;
using AMNOne.Services.Notifications.Application.Telemetry;
using AMNOne.Services.Notifications.Hub.Extensions;
using AMNOne.Services.Notifications.Hub.Hubs;
using AMNOne.Services.Notifications.Hub.Mapping;
using AMNOne.Services.Notifications.Infrastructure;
using AMNOne.Services.Notifications.Infrastructure.Configuration;
using AMNOne.Services.Notifications.Infrastructure.Extensions;
using AMNOne.Services.Notifications.Infrastructure.Services;
using AutoMapper;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AMNOne.Services.Notifications.Hub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            var aiKey = Configuration[KeyVaultSecretNames.AppInsightsInstrumentationKey];
            services.AddApplicationInsightsTelemetry(aiKey);

            services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));


            services.AddHealthChecks();

            //bind all of the option files
            services.AddOptionConfiguration();

            //Allow MediatR to use reflection to load mediator topology
            services.AddMediatR();

            //Add all of the automappers
            services.AddAutoMapperProfiles();

            //Add any custom mapper for this presentation
            services.AddAutoMapper(typeof(HubProfile));

            services.ConfigureHttpClient<ITemplateService>(HttpClientCollections.TemplateService, Configuration["Application:TemplateServiceUrl"], null, new ResiliencySettings(1, 1));

            //Add all of the DataAccess services
            services.AddServices();


            ////*******************************************************************
            ////JN-NOTE This should not be in startup but inside of Infrastructure
            ////*******************************************************************
            //services.ConfigureClient<ITemplateService, TemplateService>(Configuration["Application:TemplateServiceUrl"],
            //    null,
            //    resiliencySettings: new ResiliencySettings());

            //Instantiate a Cosmos client and add the database to DI
            //var cosmosConnectionString = Configuration[KeyVaultSecretNames.NotificationCosmosConnStr];
            //var cosmosDatabaseName = Configuration["Cosmos:DatabaseName"];

            //services.AddCosmos(cosmosConnectionString, cosmosDatabaseName);

            services.AddCors(options =>
            {
                options.AddPolicy("localhost",
                builder =>
                {
                    builder.WithOrigins("*")
                    .WithMethods("PUT", "DELETE", "GET", "POST")
                    .AllowAnyHeader();
                });
            });

            services.AddControllers();

            //Pull SignalR Service Connection string from KeyVault
            //var signalRConnectionString = Configuration[KeyVaultSecretNames.SignalRConnStr];
            var signalRConnectionString = Configuration["signalr-connstr"];

            services.AddSignalR().AddAzureSignalR(signalRConnectionString);

            var cosmosConnectionString = Configuration[KeyVaultSecretNames.NotificationCosmosConnStr];
            var cosmosDatabaseName = Configuration["Cosmos:DatabaseName"];

            services.AddCosmos(cosmosConnectionString, cosmosDatabaseName);

            var serviceName = Configuration["CognitiveSearch:ServiceName"];
            var apiKey = Configuration[KeyVaultSecretNames.CognitiveSearchApiKey];
            services.AddCognitiveSearchService(serviceName, apiKey);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCors("localhost");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseExceptionHandlerMiddleware();
            app.UseAuthorization();


            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<AlertHub>("/alert");
            });

        }

    }
}
