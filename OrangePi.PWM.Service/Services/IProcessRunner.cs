using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.PWM.Service.Services
{
    public interface IProcessRunner
    {
        Task<string> Run(string command, params string[] args);
    }
}
