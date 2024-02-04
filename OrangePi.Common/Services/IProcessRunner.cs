using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Services
{
    public interface IProcessRunner
    {
        Task<string> RunAsync(string command,params string[] args);
        Task<string> RunAsync(string command, string workingFolder, params string[] args);
        Task<T> RunAsync<T>(string command, string workingFolder, params string[] args);
        Task<T> RunAsync<T>(string command, params string[] args);
        
        Task<string> RunAsync(string command, string arguments);
        Task<string> RunAsync(string command, string workingFolder, string arguments);
        Task<T> RunAsync<T>(string command, string arguments);
        Task<T> RunAsync<T>(string command, string workingFolder, string arguments);
    }
}
