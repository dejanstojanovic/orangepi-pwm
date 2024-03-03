using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Models
{
    public class BuzzerConfig
    {
        public int PinNumber { get; set; }
        public double Frequency { get; set; }
        public int Duration { get; set; }
    }
}
