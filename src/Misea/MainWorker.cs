using Misea.Controllers;
using Misea.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misea
{
    public class MainWorker : BackgroundService
    {
        private readonly ILogger<MainWorker> logger;
        private readonly IService service;
        private readonly IActionsController actionsController;

        public MainWorker(ILogger<MainWorker> logger, IService service, IActionsController actionsController)
        {
            this.logger = logger;
            this.service = service;
            this.actionsController = actionsController;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"Misea application started");

            service.MessageReceived += MessageReceived;

            await service.Start();

            actionsController.Initialize();

            while (!stoppingToken.IsCancellationRequested)
            {
                await actionsController.PerformUpdate();
                await Task.Delay(1000, stoppingToken);
            }

            logger.LogInformation($"Got process exit command, terminating the application");
            await service.Stop();
        }

        private void MessageReceived(string message)
        {
            Task.Run(() =>
            {
                try
                {
                    actionsController.ParseMessageAndInvokeAction(message);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, $"Error occured while handling message '{message}'");
                    service.SendMessage(exception.Message);
                }
            });
        }
    }
}
