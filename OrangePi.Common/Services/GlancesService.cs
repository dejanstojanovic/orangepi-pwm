using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OrangePi.Common.Models;

namespace OrangePi.Common.Services
{
    public class GlancesService : IGlancesService
    {
        readonly Uri _glancesApi;
        private readonly IHttpClientFactory _httpClientFactory;

        public GlancesService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CpuStats> GetCpuUsage()
        {
            var httpClient = _httpClientFactory.CreateClient("glances");
            var httpResponseMessage = await httpClient.GetAsync("api/3/cpu");
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CpuStats>(contentStream);
        }

        public async Task<MemStats> GetMemoryUsage()
        {
            var httpClient = _httpClientFactory.CreateClient("glances");
            var httpResponseMessage = await httpClient.GetAsync("api/3/mem");
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<MemStats>(contentStream);
        }
    }
}
