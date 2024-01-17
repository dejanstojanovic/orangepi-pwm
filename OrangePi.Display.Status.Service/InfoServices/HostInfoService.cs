using OrangePi.Common.Services;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class HostInfoService : IHostInfoService
    {
        readonly IProcessRunner _processRunner;

        public HostInfoService(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }
        public async Task<string> GetHostName()
        {
            var output = await _processRunner.RunAsync("/bin/bash", $"-c \"hostname\"");
            return output;
        }

        public async Task<string> GetIpAddress(string adapter)
        {
            var command = $"ip -f inet addr show {adapter} | awk '/inet/ {{print $2}}' | cut -d/ -f1";
            var output = await _processRunner.RunAsync("/bin/bash", $"-c \"{command}\"");
            return output;
        }
    }
}
