using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrangePi.Common.Models
{
    public class FileSystemStats
    {
        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; }

        [JsonPropertyName("fs_type")]
        public string FsType { get; set; }

        [JsonPropertyName("mnt_point")]
        public string MntPoint { get; set; }

        [JsonPropertyName("size")]
        public object Size { get; set; }

        [JsonPropertyName("used")]
        public object Used { get; set; }

        [JsonPropertyName("free")]
        public object Free { get; set; }

        [JsonPropertyName("percent")]
        public double Percent { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
