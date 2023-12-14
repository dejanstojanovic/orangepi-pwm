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
        public bool Rotate { get; set; }
        public int TimeOn { get; set; }

        public TimeSpan TimeOnTimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds(TimeOn);
            }
        }

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
