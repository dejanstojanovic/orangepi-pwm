﻿namespace OrangePi.PWM.Service.Models
{
    public class ServiceConfiguration
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ServiceConfiguration()
        {
            this.TemperatureConfigurations = new List<TemperatureConfiguration>();
        }

        /// <summary>
        /// wPi column value from "gpio readall"
        /// </summary>
        public int wPi { get; set; }

        /// <summary>
        /// List of speed configurations for each temperature range
        /// </summary>
        public IEnumerable<TemperatureConfiguration> TemperatureConfigurations { get; set; }

        /// <summary>
        /// Interval for checking the temperature
        /// </summary>
        public int IntervalSeconds { get; set; }

    }
}
