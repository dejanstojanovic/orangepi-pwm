using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Services
{
    public interface ITemperatureService
    {
        Task<double> GetCpuTemperature();
    }
}
