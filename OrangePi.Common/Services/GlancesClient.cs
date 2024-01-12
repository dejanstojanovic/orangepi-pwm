using System.Text.Json;
using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public class GlancesClient : IGlancesClient
    {
        private readonly HttpClient _httpClient;

        public GlancesClient(HttpClient httpClient)
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

        public async Task<FileSystemStats> GetFileSystemUsage(string mountPoint)
        {
            var httpResponseMessage = await _httpClient.GetAsync("api/3/fs");
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var result =  await JsonSerializer.DeserializeAsync<IEnumerable<FileSystemStats>>(contentStream);

            return result.SingleOrDefault(r => r.MntPoint == mountPoint);
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
