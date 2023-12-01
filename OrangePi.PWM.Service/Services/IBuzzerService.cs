using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.PWM.Service.Services
{
    public interface IBuzzerService:IDisposable
    {
        int GPIO { get; init; }
        Task Play(int frequency, TimeSpan lenght);
        Task Play(int frequency, TimeSpan lenght, TimeSpan pause, int times);
    }
}
