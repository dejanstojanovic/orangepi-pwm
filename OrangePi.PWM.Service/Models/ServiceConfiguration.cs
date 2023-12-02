namespace OrangePi.PWM.Service.Models
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
        /// Buzzer GPIO pin
        /// </summary>
        public int BuzzerPin { get; set; }

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

        /// <summary>
        /// Value to set to PWM when service is shutting down
        /// </summary>
        public int ExitValue { get; set; }
    }
}
