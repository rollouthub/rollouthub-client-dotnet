using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Toggle.Client
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);
        public HttpClient Create(Uri toggleApiUri, string sdkKey)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = toggleApiUri;
            httpClient.Timeout = TimeOut;

            ConfigureHeaders(httpClient, sdkKey);

            return httpClient;
        }

        private void ConfigureHeaders(HttpClient httpClient, string sdkKey)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sdkKey);
        }
    }
}