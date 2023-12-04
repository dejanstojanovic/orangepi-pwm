using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrangePi.Common.Models
{
    public class CpuStats
    {
        [JsonPropertyName("total")]
        public double Total { get; set; }

        [JsonPropertyName("user")]
        public double User { get; set; }

        [JsonPropertyName("nice")]
        public double Nice { get; set; }

        [JsonPropertyName("system")]
        public double System { get; set; }

        [JsonPropertyName("idle")]
        public double Idle { get; set; }

        [JsonPropertyName("iowait")]
        public double Iowait { get; set; }

        [JsonPropertyName("irq")]
        public double Irq { get; set; }

        [JsonPropertyName("softirq")]
        public double Softirq { get; set; }

        [JsonPropertyName("steal")]
        public double Steal { get; set; }

        [JsonPropertyName("guest")]
        public double Guest { get; set; }

        [JsonPropertyName("guest_nice")]
        public double GuestNice { get; set; }

        [JsonPropertyName("time_since_update")]
        public double TimeSinceUpdate { get; set; }

        [JsonPropertyName("cpucore")]
        public double Cpucore { get; set; }

        [JsonPropertyName("ctx_switches")]
        public double CtxSwitches { get; set; }

        [JsonPropertyName("doubleerrupts")]
        public double doubleerrupts { get; set; }

        [JsonPropertyName("soft_doubleerrupts")]
        public double Softdoubleerrupts { get; set; }

        [JsonPropertyName("syscalls")]
        public double Syscalls { get; set; }
    }


}
