using Iot.Device.CpuTemperature;

namespace OrangePi.Common.Services
{
    public class CpuTemperatureReader : ITemperatureReader
    {
        private readonly IProcessRunner _processRunner;
        private readonly CpuTemperature _cpuTemperature;

        public CpuTemperatureReader(
            IProcessRunner processRunner)
        {
            _processRunner = processRunner;
            _cpuTemperature = new CpuTemperature();
        }

        public CpuTemperatureReader()
        {
                
        }
        public async Task<double> GetTemperature()
        {
            double temperature = 0;
            if (!double.IsNaN(_cpuTemperature.Temperature.DegreesCelsius) && _cpuTemperature.Temperature.DegreesCelsius > 0)
            {
                temperature = _cpuTemperature.Temperature.DegreesCelsius;
            }
            else
            {
                var temeratureCheckOutput = await _processRunner.RunAsync("cat", "/sys/class/thermal/thermal_zone0/temp");
                if (double.TryParse(temeratureCheckOutput, out double temperatureCheckValue) &&
                    temperatureCheckValue > 0)
                {
                    temperature = temperatureCheckValue / 1000;
                }
            }
            return temperature;
        }
    }
}
