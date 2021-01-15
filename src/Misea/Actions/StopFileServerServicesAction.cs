using Misea.Services;
using Misea.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Misea.Actions
{
    class StopFileServerServicesAction : IAction
    {
        public string Name => "Stop file server services";

        private readonly IService service;
        private readonly ILogger<StopFileServerServicesAction> logger;

        public StopFileServerServicesAction(ILogger<StopFileServerServicesAction> logger, IService service)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task PerformAction()
        {
            logger.LogInformation("Stopping file server services");

            await StopMiniDlnaService();
            await StopSambaService();

            logger.LogInformation("Services successfully stopped");
            await service.SendMessage("Services successfully stopped");
        }

        private async Task StopSambaService()
        {
            logger.LogInformation("Stopping smbd service");

            string output = await BashUtilities.Execute("sudo systemctl stop smbd");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }
        }

        private async Task StopMiniDlnaService()
        {
            logger.LogInformation("Stopping mini dlna service");

            string output = await BashUtilities.Execute("sudo systemctl stop minidlna");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }
        }
    }
}
