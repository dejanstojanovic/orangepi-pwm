using System.Text.Json;
using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public class GlancesService : IGlancesService
    {
        readonly Uri _glancesApi;
        private readonly HttpClient _httpClient;

        public GlancesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CpuStats> GetCpuUsage()
        {
            var httpResponseMessage = await _httpClient.GetAsync("api/3/cpu");
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CpuStats>(contentStream);
        }

        public async Task<MemStats> GetMemoryUsage()
        {
            var httpResponseMessage = await _httpClient.GetAsync("api/3/mem");
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<MemStats>(contentStream);
        }
    }
}
