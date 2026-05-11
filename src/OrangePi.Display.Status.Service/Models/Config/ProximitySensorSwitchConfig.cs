namespace OrangePi.Display.Status.Service.Models.Config
{
    public class ProximitySensorSwitchConfig
    {
        public int BusId { get; set; }
        public string DeviceAddressHex { get; set; }
        public int Distance { get; set; }

        public int DeviceAddress
        {
            get
            {
                return Convert.ToInt32(this.DeviceAddressHex, 16);
            }
        }
    }
}
