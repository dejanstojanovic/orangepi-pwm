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
        public int PauseSeconds { get; set; }

        public TimeSpan Pause
        {
            get
            {
                return TimeSpan.FromSeconds(PauseSeconds);
            }
        }

        public int DeviceAddress { get
            {
                return Convert.ToInt32(this.DeviceAddressHex, 16);
            } 
        }
    }
}
