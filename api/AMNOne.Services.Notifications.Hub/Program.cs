using AMNOne.Framework.ExceptionHandling.NLogLogging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using System;
using System.Diagnostics;
using System.IO;

namespace AMNOne.Services.Notifications.Hub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     var builtConfig = config.Build();
                     var vaultUrl = $"https://{builtConfig["AzureKeyVaultName"]}.vault.azure.net/";

                     if (context.HostingEnvironment.IsDevelopment()) //Use appid & secret from user secrets
                     {
                         config.AddAzureKeyVault(vaultUrl, builtConfig["AzureKeyVault:applicationid"], builtConfig["AzureKeyVault:secret"]);
                     }
                     else //Use Managed Identity
                     {
                         var azureServiceTokenProvider = new AzureServiceTokenProvider();
                         var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                         config.AddAzureKeyVault(vaultUrl, keyVaultClient, new DefaultKeyVaultSecretManager());
                     }
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(ConfigureLogging);
                    webBuilder.UseStartup<Startup>();
                });


        public static void ConfigureLogging(WebHostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            string logPath = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "nlog.config");
            if (!File.Exists(logPath))
            {
                throw new MissingMemberException($"Missing NLog configuration file '{logPath}'");
            }
            var nLoggingConfiguration = new XmlLoggingConfiguration(logPath);

            var logJsonCgf = hostingContext.Configuration.GetSection(nameof(NLogLoggerSettings));
            if (!logJsonCgf.Exists())
            {
                throw new MissingMemberException($"Missing configuration section '{nameof(NLogLoggerSettings)}'");
            }

            logging.ClearProviders();
            logging.AddNLogLogger(logJsonCgf, nLoggingConfiguration);
            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        }
    }
}
