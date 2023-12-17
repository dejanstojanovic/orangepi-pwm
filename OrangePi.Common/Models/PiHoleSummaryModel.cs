using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrangePi.Common.Models
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class GravityLastUpdated
    {
        [JsonPropertyName("file_exists")]
        public bool FileExists { get; set; }

        [JsonPropertyName("absolute")]
        public int Absolute { get; set; }

        [JsonPropertyName("relative")]
        public Relative Relative { get; set; }
    }

    public class Relative
    {
        [JsonPropertyName("days")]
        public int Days { get; set; }

        [JsonPropertyName("hours")]
        public int Hours { get; set; }

        [JsonPropertyName("minutes")]
        public int Minutes { get; set; }
    }

    public class PiHoleSummaryModel
    {
        [JsonPropertyName("domains_being_blocked")]
        public int DomainsBeingBlocked { get; set; }

        [JsonPropertyName("dns_queries_today")]
        public int DnsQueriesToday { get; set; }

        [JsonPropertyName("ads_blocked_today")]
        public int AdsBlockedToday { get; set; }

        [JsonPropertyName("ads_percentage_today")]
        public double AdsPercentageToday { get; set; }

        [JsonPropertyName("unique_domains")]
        public int UniqueDomains { get; set; }

        [JsonPropertyName("queries_forwarded")]
        public int QueriesForwarded { get; set; }

        [JsonPropertyName("queries_cached")]
        public int QueriesCached { get; set; }

        [JsonPropertyName("clients_ever_seen")]
        public int ClientsEverSeen { get; set; }

        [JsonPropertyName("unique_clients")]
        public int UniqueClients { get; set; }

        [JsonPropertyName("dns_queries_all_types")]
        public int DnsQueriesAllTypes { get; set; }

        [JsonPropertyName("reply_UNKNOWN")]
        public int ReplyUNKNOWN { get; set; }

        [JsonPropertyName("reply_NODATA")]
        public int ReplyNODATA { get; set; }

        [JsonPropertyName("reply_NXDOMAIN")]
        public int ReplyNXDOMAIN { get; set; }

        [JsonPropertyName("reply_CNAME")]
        public int ReplyCNAME { get; set; }

        [JsonPropertyName("reply_IP")]
        public int ReplyIP { get; set; }

        [JsonPropertyName("reply_DOMAIN")]
        public int ReplyDOMAIN { get; set; }

        [JsonPropertyName("reply_RRNAME")]
        public int ReplyRRNAME { get; set; }

        [JsonPropertyName("reply_SERVFAIL")]
        public int ReplySERVFAIL { get; set; }

        [JsonPropertyName("reply_REFUSED")]
        public int ReplyREFUSED { get; set; }

        [JsonPropertyName("reply_NOTIMP")]
        public int ReplyNOTIMP { get; set; }

        [JsonPropertyName("reply_OTHER")]
        public int ReplyOTHER { get; set; }

        [JsonPropertyName("reply_DNSSEC")]
        public int ReplyDNSSEC { get; set; }

        [JsonPropertyName("reply_NONE")]
        public int ReplyNONE { get; set; }

        [JsonPropertyName("reply_BLOB")]
        public int ReplyBLOB { get; set; }

        [JsonPropertyName("dns_queries_all_replies")]
        public int DnsQueriesAllReplies { get; set; }

        [JsonPropertyName("privacy_level")]
        public int PrivacyLevel { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("gravity_last_updated")]
        public GravityLastUpdated GravityLastUpdated { get; set; }
    }


}
