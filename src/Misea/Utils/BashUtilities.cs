using System.Diagnostics;
using System.Threading.Tasks;

namespace Misea.Utils
{
    internal static class BashUtilities
    {
        internal static async Task<string> Execute(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            result += process.StandardError.ReadToEnd();

            await process.WaitForExitAsync();

            return result;
        }
    }
}
