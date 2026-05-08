namespace OrangePi.Display.Status.Service.Services.Switch
{
    public class ProximitySensorSwitch : ISwitch
    {
        private bool _isOn = true;

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
