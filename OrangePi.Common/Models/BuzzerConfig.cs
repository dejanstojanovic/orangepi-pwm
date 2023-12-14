using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Models
{
    public class BuzzerConfig
    {
        public int GPIO { get; set; }
        public int PWMFrequency { get; set; }
        public double PWMDuty { get; set; }
    }
}
