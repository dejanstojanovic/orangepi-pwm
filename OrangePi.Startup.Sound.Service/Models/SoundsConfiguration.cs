using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Startup.Sound.Service.Models
{
    public class SoundsConfiguration
    {
        public SoundConfiguration Startup { get; set; }
        public SoundConfiguration Shutdown { get; set; }
    }
}
