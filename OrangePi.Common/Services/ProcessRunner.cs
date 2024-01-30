using System.Diagnostics;
using System.Text;

namespace OrangePi.Common.Services
{
    public class ProcessRunner : IProcessRunner
    {
        private async Task<String> getOutput(Process process)
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            await process.WaitForExitAsync();

            return output;
        }
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
                return await getOutput(process);
            }
        }

        public async Task<T> RunAsync<T>(string command, params string[] args)
        {
            var result = await RunAsync(command, args);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }

        public async Task<string> RunAsync(string command, string arguments, bool useShellExecute = false)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = useShellExecute;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;

                return await getOutput(process);
            }
        }



        public async Task<T> RunAsync<T>(string command, string arguments, bool useShellExecute = false)
        {
            var result = await RunAsync(command,arguments,useShellExecute);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }
    }
}
