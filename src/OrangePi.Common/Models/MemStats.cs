using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrangePi.Common.Models
{
    public class MemStats
    {
        [JsonPropertyName("total")]
        public double Total { get; set; }

        [JsonPropertyName("available")]
        public double Available { get; set; }

        [JsonPropertyName("percent")]
        public double Percent { get; set; }

        [JsonPropertyName("used")]
        public double Used { get; set; }

        [JsonPropertyName("free")]
        public double Free { get; set; }

        [JsonPropertyName("active")]
        public double Active { get; set; }

        [JsonPropertyName("inactive")]
        public double Inactive { get; set; }

        [JsonPropertyName("buffers")]
        public double Buffers { get; set; }

        [JsonPropertyName("cached")]
        public double Cached { get; set; }

        [JsonPropertyName("shared")]
        public double Shared { get; set; }
    }


}
