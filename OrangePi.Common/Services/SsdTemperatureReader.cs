namespace OrangePi.Common.Services
{
    public class SsdTemperatureReader : ITemperatureReader
    {
        readonly IProcessRunner _processRunner;
        public SsdTemperatureReader(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task<double> GetTemperature()
        {
            var output = await _processRunner.RunAsync("/bin/bash", "-c \"smartctl -a /dev/nvme0 | grep 'Temperature:'\"", false);
            var value = output.Split(":").Last().Replace("Celsius", string.Empty).Trim();
            return double.Parse(value);
        }
    }
}
