using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Services
{
    public interface ITemperatureReader
    {
        Task<double> GetTemperature();
    }
}
