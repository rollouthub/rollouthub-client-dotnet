using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleJson;
using Toggle.Logging;

namespace Toggle.Client
{
    public class FeatureRequestor : IFeatureRequestor
    {
        private static readonly ILog Logger = LogProvider.GetLogger(typeof(FeatureRequestor));

        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;
        private readonly Uri _allTogglesUri;

        public FeatureRequestor(Configuration configuration)
        {
            _configuration = configuration;
            _httpClient = configuration.HttpClientFactory.Create(configuration.ToggleApi, configuration.SdkKey);
            _allTogglesUri = new Uri(configuration.ToggleApi + "client/toggles");
        }

        public async Task<TogglesResult> GetAll(CancellationToken cancellationToken)
        {
            Logger.Trace($"Toggle: Get all toggles request");
            
            var request = new HttpRequestMessage(HttpMethod.Get, _allTogglesUri);

            var responseString = await MakeRequest(request, cancellationToken);
            
            var jsonDataDictionary = (IDictionary<string, object>) SimpleJson.SimpleJson.DeserializeObject(responseString);
            var toggles = (JsonArray)jsonDataDictionary["toggles"];
            if (toggles == null)
            {
                return new TogglesResult
                {
                    Modified = false
                };
            }
            
            var toggleList = new List<FeatureToggle>(); 
            foreach (var toggle in toggles)
            {
                var item = toggle as JsonObject;
                var featureToggle = new FeatureToggle((string)item["key"], (string)item["name"], (bool)item["on"]);
                toggleList.Add(featureToggle);
            }
            var toggleCollection = new ToggleCollection(toggleList);

            return new TogglesResult
            {
                Modified = true,
                ToggleCollection = toggleCollection
            };
        }

        private async Task<string> MakeRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Logger.Trace($"Toggle: Error fetching all toggles with response code {response.StatusCode}: " + error);
                    
                    throw new UnsuccessfulResponseFromApiException((int)response.StatusCode);
                }

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}