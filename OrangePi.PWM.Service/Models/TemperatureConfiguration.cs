using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.PWM.Service.Models
{
    public class TemperatureConfiguration
    {
        /// <summary>
        /// Temperature threshold for which speed is applied
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// PWM speed value to be set for Temperature threshold
        /// </summary>
        public int Speed { get; set; }
    }
}
