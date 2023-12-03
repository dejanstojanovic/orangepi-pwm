using System.Text;

namespace OrangePi.Display.Status.Service.Models
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            
        }

        public int BusId { get; set; }
        public string DeviceAddressHex { get; set; }
        public int Interval { get; set; }

        public string FontName { get; set; }
        public int FontSize { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public TimeSpan IntervalTimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds(Interval);
            }
        }

        public int DeviceAddress { get
            {
                return Convert.ToInt32(this.DeviceAddressHex, 16);
            } 
        }
    }
}
