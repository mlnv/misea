using Misea.Services;
using Misea.Utils;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

namespace Misea.Actions
{
    class ShowTemperatureAction : IAction
    {
        public string Name => "Show temperature";

        private readonly IService service;
        private readonly ILogger<ShowTemperatureAction> logger;

        public ShowTemperatureAction(ILogger<ShowTemperatureAction> logger, IService service)
        {
            this.service = service;
            this.logger = logger;
        }

        public async Task PerformAction()
        {
            logger.LogInformation("Running get raspbery temperature action");

            float cpuTemperatureCelsius = await GetCPUTemperatureCelsius();
            float gpuTemperatureCelsius = await GetGPUTemperatureCelsius();

            await service.SendMessage($"CPU: {cpuTemperatureCelsius:0.00}'C \n" +
                $"GPU: {gpuTemperatureCelsius:0.00}'C");
        }

        private async Task<float> GetCPUTemperatureCelsius()
        {
            string cpuTemperatureCommandOutput = await BashUtilities.Execute("cat /sys/class/thermal/thermal_zone0/temp");

            if (string.IsNullOrEmpty(cpuTemperatureCommandOutput))
            {
                return -1;
            }

            return float.Parse(cpuTemperatureCommandOutput, CultureInfo.InvariantCulture.NumberFormat) / 1000;
        }

        private async Task<float> GetGPUTemperatureCelsius()
        {
            string gpuTemperatureCommandOutput = await BashUtilities.Execute("vcgencmd measure_temp");

            // Example output: temp=48.3'C, we need to remove first 5 and last 2 characters to get the number
            gpuTemperatureCommandOutput = gpuTemperatureCommandOutput.Remove(0, 5);
            gpuTemperatureCommandOutput = gpuTemperatureCommandOutput.Remove(gpuTemperatureCommandOutput.Length - 3);

            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

            float gpuTemperatureCelsius = float.Parse(gpuTemperatureCommandOutput, NumberStyles.Any, cultureInfo);
            return gpuTemperatureCelsius;
        }
    }
}
