using System.Diagnostics;

namespace OrangePi.Common.Services
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<string> RunAsync(string command, params string[] args)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                foreach (var arg in args)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                await process.WaitForExitAsync();

                return output;
            }
        }
    }
}
