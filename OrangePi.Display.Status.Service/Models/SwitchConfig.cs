using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.Models
{
    public class SwitchConfig
    {
        public int GPIO { get; set; }
        public int Interval { get; set; }
        public TimeSpan IntervalTimeSpan { get => TimeSpan.FromMicroseconds(Interval); }
    }
}
