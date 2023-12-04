using OrangePi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Services
{
    public interface IGlancesService
    {
        Task<CpuStats> GetCpuUsage();
        Task<MemStats> GetMemoryUsage();
    }
}
