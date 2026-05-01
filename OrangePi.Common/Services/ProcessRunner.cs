using System.Diagnostics;

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

        public async Task<string> WaitForOutputAsync(string command, string workingFolder, params string[] args)
        {
            using (Process process = new Process())
            {
                if (workingFolder != null)
                    process.StartInfo.WorkingDirectory = workingFolder;

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

        public async Task<string> WaitForOutputAsync(string command, string workingFolder, string arguments)
        {
            using (Process process = new Process())
            {
                if (workingFolder != null)
                    process.StartInfo.WorkingDirectory = workingFolder;

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;

                return await getOutput(process);
            }
        }


        public async Task<string> WaitForOutputAsync(string command, params string[] args)
        {
            return await WaitForOutputAsync(command, null, args);
        }

        public async Task<T> WaitForOutputAsync<T>(string command, params string[] args)
        {
            var result = await WaitForOutputAsync(command, args);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }

        public async Task<string> WaitForOutputAsync(string command, string arguments)
        {
            return await WaitForOutputAsync(command, null, arguments);
        }

        public async Task<T> WaitForOutputAsync<T>(string command, string arguments)
        {
            var result = await WaitForOutputAsync(command, arguments);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }

        public async Task<T> WaitForOutputAsync<T>(string command, string workingFolder, params string[] args)
        {
            var result = await WaitForOutputAsync(command, workingFolder, args);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }

        public async Task<T> WaitForOutputAsync<T>(string command, string workingFolder, string arguments)
        {
            var result = await WaitForOutputAsync(command, workingFolder, arguments);
            return (T)System.Convert.ChangeType(result, typeof(T));
        }

        public async Task RunAsync(string command, params string[] args)
        {
            await RunAsync(command, null, args);
        }

        public async Task RunAsync(string command, string arguments)
        {
            await RunAsync(command, null, arguments);
        }

        public async Task RunAsync(string command, string workingFolder, params string[] args)
        {
            using (Process process = new Process())
            {
                if (workingFolder != null)
                    process.StartInfo.WorkingDirectory = workingFolder;

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;

                foreach (var arg in args)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }
                process.Start();
            }
        }

        public async Task RunAsync(string command, string workingFolder, string arguments)
        {
            using (Process process = new Process())
            {
                if (workingFolder != null)
                    process.StartInfo.WorkingDirectory = workingFolder;

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;

                process.Start();
            }
        }
    }
}
