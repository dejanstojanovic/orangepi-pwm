using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public interface IGlancesClient
    {
        Task<CpuStats> GetCpuUsage();
        Task<MemStats> GetMemoryUsage();
        Task<FileSystemStats> GetFileSystemUsage(string mountPoint);
    }
}
