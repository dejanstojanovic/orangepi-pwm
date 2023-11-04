using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.PWM.Service.Models
{
    public class ServiceConfiguration
    {
        /// <summary>
        /// wPi column value from "gpio readall"
        /// </summary>
        public int wPi { get; set; }

        /// <summary>
        /// List of speed configurations for each temperature range
        /// </summary>
        public IEnumerable<TemperatureConfiguration> TemperatureConfigurations { get; set; }

    }
}
