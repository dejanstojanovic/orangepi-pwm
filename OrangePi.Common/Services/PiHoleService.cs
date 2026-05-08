using Microsoft.Extensions.Options;
using OrangePi.Common.Models;
using System.Text.Json;
using Flurl;

namespace OrangePi.Common.Services
{
    public class PiHoleService : IPiHoleService
    {
        private readonly HttpClient _httpClient;
        private readonly PiHoleConfig _config;
        public PiHoleService(HttpClient httpClient, IOptions<PiHoleConfig> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<PiHoleSummaryModel> GetSummary()
        {
            var httpResponseMessage = await _httpClient.GetAsync("admin/api.php"
                                                        .AppendQueryParam(name:"auth", value: _config.Key)
                                                        .AppendQueryParam(name: "summaryRaw"));
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<PiHoleSummaryModel>(contentStream);
        }
    }
}
