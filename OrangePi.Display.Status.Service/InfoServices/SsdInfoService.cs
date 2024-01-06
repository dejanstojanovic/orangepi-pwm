using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class SsdInfoService : IInfoService
    {
        private readonly IGlancesClient _glancesService;
        private readonly IProcessRunner _processRunner;
        public SsdInfoService(
            IGlancesClient glancesService,
            IProcessRunner processRunner)
        {
            _glancesService = glancesService;
            _processRunner = processRunner;

        }

        public string Label => "SSD";

        public async Task<StatusValue> GetValue()
        {
            double fsUsage = 0;
            try
            {
                var fsUsageModel = await _glancesService.GetFileSystemUsage("/etc/hostname");
                fsUsage = Math.Round(fsUsageModel.Percent, 2);
            }
            catch { fsUsage = 0; }

            double ssdTemp = 0;
            try
            {
                var output = await _processRunner.RunAsync("/bin/bash", "-c \"smartctl -a /dev/nvme0 | grep 'Temperature:'\"", false);
                var value = output.Split(":").Last().Replace("Celsius", string.Empty).Trim();
                ssdTemp = double.Parse(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: {ex.Message}");
                ssdTemp = 0;
            }


            return new StatusValue(
                valueText: $"{fsUsage}%",
                value: fsUsage,
                note: $"{ssdTemp}°C");
        }
    }
}
