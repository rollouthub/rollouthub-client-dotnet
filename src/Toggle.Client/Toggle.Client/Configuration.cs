using System;

namespace Toggle.Client
{
    public class Configuration
    {
        public string SdkVersion { get; internal set; }
        
        public Uri ToggleApi { get; set; } = new Uri("http://localhost:5000"); 
        
        public string SdkKey { get; internal set; }
        
        public TimeSpan PollingInterval { get; internal set; }
        
        public IHttpClientFactory HttpClientFactory { get; internal set; }

        public static Configuration Default(string sdkKey)
        {
            var configuration = new Configuration
            {
                SdkVersion = "0.1",
                SdkKey = sdkKey,
                PollingInterval = TimeSpan.FromSeconds(10),
                HttpClientFactory = new HttpClientFactory()
            };

            return configuration;
        }
    }
}