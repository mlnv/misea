using Misea.Services;
using Misea.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Misea.Actions
{
    class MountUSBDriveAction : IAction
    {
        public string Name => "Mount USB drive";

        private readonly IService service;
        private readonly ILogger<MountUSBDriveAction> logger;

        public MountUSBDriveAction(ILogger<MountUSBDriveAction> logger, IService service)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task PerformAction()
        {
            logger.LogInformation("Mounting USB drive");

            string output = await BashUtilities.Execute("sudo mount /dev/sda2 /media/usb-drive");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }

            logger.LogInformation("USB drive successfully mounted");
            await service.SendMessage("USB drive successfully mounted");
        }
    }
}
