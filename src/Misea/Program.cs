using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Misea.Actions;
using Misea.Controllers;
using Misea.Options;
using Misea.Services;
using NLog.Web;

namespace Misea
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<TelegramServiceOptions>()
                        .Bind(hostContext.Configuration.GetSection(TelegramServiceOptions.TelegramService))
                        .ValidateDataAnnotations();

                    services.AddHostedService<MainWorker>();

                    services.AddSingleton<IActionsController, ActionsController>();
                    services.AddSingleton<IService, TelegramService>();

                    services.AddSingleton<IAction, ShowTemperatureAction>();
                    services.AddSingleton<IAction, StartFileServerServicesAction>();
                    services.AddSingleton<IAction, StopFileServerServicesAction>();
                    services.AddSingleton<IAction, MountUSBDriveAction>();
                    services.AddSingleton<IAction, UnmountUSBDriveAction>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}
