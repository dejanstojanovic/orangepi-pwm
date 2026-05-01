namespace OrangePi.Common.Services
{
    public interface IProcessRunner
    {
        Task<string> WaitForOutputAsync(string command,params string[] args);
        Task<string> WaitForOutputAsync(string command, string workingFolder, params string[] args);
        Task<T> WaitForOutputAsync<T>(string command, string workingFolder, params string[] args);
        Task<T> WaitForOutputAsync<T>(string command, params string[] args);
        Task<string> WaitForOutputAsync(string command, string arguments);
        Task<string> WaitForOutputAsync(string command, string workingFolder, string arguments);
        Task<T> WaitForOutputAsync<T>(string command, string arguments);
        Task<T> WaitForOutputAsync<T>(string command, string workingFolder, string arguments);

        Task RunAsync(string command, params string[] args);
        Task RunAsync(string command, string workingFolder, params string[] args);
        Task RunAsync(string command, string arguments);
        Task RunAsync(string command, string workingFolder, string arguments);
    }
}
