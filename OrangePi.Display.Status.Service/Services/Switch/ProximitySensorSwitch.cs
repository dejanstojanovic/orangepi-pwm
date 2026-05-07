using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.Services.Switch
{
    public class ProximitySensorSwitch : ISwitch
    {
        private bool _isOn = false;
        public bool IsOn
        {
            get
            {
                return _isOn;
            }
            private set
            {
                _isOn = value;
            }
        }
    }
}
