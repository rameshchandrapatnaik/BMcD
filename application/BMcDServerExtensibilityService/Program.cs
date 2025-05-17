using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BMcDExtensibilityService;
using BMcDExtensibilityService.Core;
using BMcDExtensibilityService.Core.Handlers;
using BMcDExtensibilityService.Core.Services;
using BMcDExtensibilityService.Custom.Workers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMcDExtensibilityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The service failed to start..");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;

                // add logging configuration
                var loggerConfig = new ConfigurationBuilder().AddConfiguration(configuration)
                    //.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json")
                    .Build();
                
                // create logger
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(loggerConfig)
                    .CreateLogger();

                Log.Information("Extensibility Service Starting Up...");

                ExtensibilityConfiguration config = configuration.GetSection("appSettings").Get<ExtensibilityConfiguration>();
               
                services.AddSingleton(config);
                services.AddSingleton<AuthenticationService>();
                services.AddSingleton<ExtensibilityODataClient>(serviceProvider =>
                {
                    return new ExtensibilityODataClient(config, serviceProvider.GetRequiredService<AuthenticationService>(),
                        new List<string>());
                });
                services.AddSingleton<ExtensibilityRestClient>();
                services.AddSingleton<IHostedService,AuthenticationHostedService>();
                services.AddSingleton<ExtensibilityEventHandler>();
                services.AddSingleton<AMQPHandler>();
                services.AddSingleton<AzureAMQPHandler>();
                services.AddHostedService<SDxQueueWorker>();
            })
            .UseSerilog();
    }
}
