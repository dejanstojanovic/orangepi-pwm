using Iot.Device.Vl53L1X;
using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models.Config;
using System.Device.I2c;
using UnitsNet;

namespace OrangePi.Display.Status.Service.Services.Switch
{
    public class ProximitySensorSwitch : ISwitch
    {
        private bool _isOn = true;
        public event EventHandler<bool>? Changed;
        readonly object _lock = new Object();
        readonly ProximitySensorSwitchConfig _config;
        private readonly Length _triggerDistance;
        public ProximitySensorSwitch(IOptions<ProximitySensorSwitchConfig> options)
        {
            _config = options.Value;
            _triggerDistance = Length.FromMillimeters(_config.Distance);

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
                bool changed;
                lock (_lock)
                {
                    changed = _isOn != value;
                    _isOn = value;
                }
                if (changed)
                    Changed?.Invoke(this, value);
            }
        }

        public Task StartMonitoringAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                var _i2cSettings = new I2cConnectionSettings(1, Vl53L1X.DefaultI2cAddress);

                using (var _i2cDevice = I2cDevice.Create(_i2cSettings))
                {
                    using (var _distanceSensor = new Vl53L1X(_i2cDevice))
                    {
                        _distanceSensor.StartRanging();
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            if (_distanceSensor.GetDistance() <= _triggerDistance)
                            {
                                IsOn = true;
                            }
                            else
                            {
                                IsOn = false;
                            }
                            await Task.Delay(TimeSpan.FromMilliseconds(100)).WaitAsync(stoppingToken);
                        }
                        _distanceSensor.StopRanging();
                    }
                }
            });
        }
    }
}
