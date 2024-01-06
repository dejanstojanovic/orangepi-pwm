using OrangePi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Services
{
    public interface IGlancesClient
    {
        Task<CpuStats> GetCpuUsage();
        Task<MemStats> GetMemoryUsage();
        Task<FileSystemStats> GetFileSystemUsage(string mountPoint);
    }
}
