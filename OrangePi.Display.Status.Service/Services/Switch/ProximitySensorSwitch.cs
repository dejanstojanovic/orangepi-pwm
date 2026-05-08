using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models.Config;

namespace OrangePi.Display.Status.Service.Services.Switch
{
    public class ProximitySensorSwitch : ISwitch
    {
        private bool _isOn = true;
        public event EventHandler<bool>? Changed;
        readonly object _lock = new Object();
        readonly ProximitySensorSwitchConfig _config;

        public ProximitySensorSwitch(IOptions<ProximitySensorSwitchConfig> options)
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

        public async Task StartMonitoringAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
