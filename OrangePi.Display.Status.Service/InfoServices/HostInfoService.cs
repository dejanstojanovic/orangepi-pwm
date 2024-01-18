using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class HostInfoService : IHostInfoService, IDisplayInfoService
    {
        readonly IProcessRunner _processRunner;
        readonly string _networkAdapter;

        public HostInfoService(IProcessRunner processRunner, string networkAdapter)
        {
            _processRunner = processRunner;
            _networkAdapter = networkAdapter;
        }

        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

        public async Task<string> GetHostName()
        {
            var output = await _processRunner.RunAsync("/bin/bash", $"-c \"hostname\"");
            return output;
        }

        public async Task<string> GetIpAddress()
        {
            var command = $"ip -f inet addr show {_networkAdapter} | awk '/inet/ {{print $2}}' | cut -d/ -f1";
            var output = await _processRunner.RunAsync("/bin/bash", $"-c \"{command}\"");
            return output;
        }
    }
}
