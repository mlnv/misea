using Misea.Services;
using Misea.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Misea.Actions
{
    class UnmountUSBDriveAction : IAction
    {
        public string Name => "Unmount USB drive";

        private readonly IService service;
        private readonly ILogger<UnmountUSBDriveAction> logger;

        public UnmountUSBDriveAction(ILogger<UnmountUSBDriveAction> logger, IService service)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task PerformAction()
        {
            logger.LogInformation("Unmounting USB drive");

            string output = await BashUtilities.Execute("sudo umount /dev/sda2");

            if (!string.IsNullOrEmpty(output))
            {
                await service.SendMessage(output);
            }

            logger.LogInformation("USB drive successfully unmounted");
            await service.SendMessage("USB drive successfully unmounted");
        }
    }
}
