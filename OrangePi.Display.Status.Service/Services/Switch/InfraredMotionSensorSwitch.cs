using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models.Config;
using System.Device.Gpio;
namespace OrangePi.Display.Status.Service.Services.Switch
{
    public class InfraredMotionSensorSwitch : ISwitch
    {
        readonly InfraredMotionSensorSwitchConfig _config;
        private bool _isOn = true;
        readonly object _lock = new Object();

        public InfraredMotionSensorSwitch(IOptions<InfraredMotionSensorSwitchConfig> options)
        {
            _config = options.Value;
        }

        public bool IsOn
        {
            get
            {
                lock (_lock)
                {
                    return _isOn;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _isOn = value;
                }
            }
        }

        public async Task StartMonitoringAsync(CancellationToken stoppingToken)
        {
            using (var controller = new GpioController())
            {
                var pin = controller.OpenPin(_config.GPIO, PinMode.Input);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var value = pin.Read();
                    if (value == PinValue.High)
                    {
                        this.IsOn = true;
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(100)).WaitAsync(stoppingToken);
                }
            }
        }

    }
}
