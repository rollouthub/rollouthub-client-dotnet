using System;
using System.Net.Http;

namespace Toggle.Client
{
    public interface IHttpClientFactory
    {
        HttpClient Create(Uri toggleApiUri, string sdkKey);
    }
}