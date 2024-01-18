using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public interface IHostInfoService:IDisplayInfoService
    {
        Task<string> GetIpAddress();
        Task<string> GetHostName();
    }
}
