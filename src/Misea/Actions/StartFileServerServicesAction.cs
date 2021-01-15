using Misea.Services;
using Misea.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Misea.Actions
{
    class StartFileServerServicesAction : IAction
    {
        public string Name => "Start file server services";

        private readonly IService service;
        private readonly ILogger<StartFileServerServicesAction> logger;

        public StartFileServerServicesAction(ILogger<StartFileServerServicesAction> logger, IService service)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task PerformAction()
        {
            logger.LogInformation("Starting file server services");

            await StartMiniDlnaService();
            await StartSambaService();

            logger.LogInformation("Services successfully started");
            await service.SendMessage("Services successfully started");
        }

        private async Task StartSambaService()
        {
            logger.LogInformation("Starting smbd service");

            string output = await BashUtilities.Execute("sudo systemctl start smbd");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }
        }

        private async Task StartMiniDlnaService()
        {
            logger.LogInformation("Starting mini dlna service");

            string output = await BashUtilities.Execute("sudo systemctl start minidlna");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }
        }
    }
}
