namespace OrangePi.Common.Services
{
    public class SsdTemperatureReader : ITemperatureReader
    {
        readonly IProcessRunner _processRunner;
        readonly string _drive;
        public SsdTemperatureReader(
            IProcessRunner processRunner, 
            string drive)
        {
            _processRunner = processRunner;
            _drive = drive;
        }

        public async Task<double> GetTemperature()
        {
            var output = await _processRunner.RunAsync("/bin/bash", $"-c \"smartctl -a /dev/{_drive} | grep 'Temperature:'\"", false);
            var value = output.Split(":").Last().Replace("Celsius", string.Empty).Trim();
            return double.Parse(value);
        }
    }
}
