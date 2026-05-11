using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Fan.Service.Models
{
    public class TemperatureConfiguration
    {
        /// <summary>
        /// Temperature threshold for which speed is applied
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// PWM value to be set for Temperature threshold
        /// </summary>
        public int Value { get; set; }
    }
}
