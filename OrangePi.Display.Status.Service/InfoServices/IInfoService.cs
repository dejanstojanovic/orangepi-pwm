using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public interface IInfoService
    {
        Task<StatusValue> GetValue();
        string Label {  get; }
    }
}
