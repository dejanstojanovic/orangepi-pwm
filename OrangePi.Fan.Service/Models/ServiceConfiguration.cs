namespace OrangePi.Fan.Service.Models
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
        public int WiringPi { get; set; }

        /// <summary>
        /// List of speed configurations for each temperature range
        /// </summary>
        public IEnumerable<TemperatureConfiguration> TemperatureConfigurations { get; set; }

        /// <summary>
        /// Interval for checking the temperature
        /// </summary>
        public double TemperatureCheckInterval { get; set; }

        /// <summary>
        /// Value to set to PWM when service is shutting down
        /// </summary>
        public int ValueToSetOnExit { get; set; }

        public SoundConfig StartSound { get; set; }
        public SoundConfig ExitSound { get; set; }
    }
}
