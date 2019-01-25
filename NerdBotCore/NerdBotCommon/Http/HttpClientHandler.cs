using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NerdBotCommon.Extensions;

namespace NerdBotCommon.Http
{
    public class HttpClientHandler : IHttpHandler
    {
        private readonly HttpClient _httpClient;

        public HttpClientHandler()
        {
            this._httpClient = new HttpClient();
        }

        public HttpResponseMessage Get(string url)
        {
            return GetAsync(url).Result;
        }

        public string GetString(string url)
        {
            return GetStringAsync(url).Result;
        }

        public HttpResponseMessage Post(string url, HttpContent content)
        {
            return PostAsync(url, content).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await this._httpClient.GetAsync(url);
        }

        public async Task<string> GetStringAsync(string url)
        {
            return await this._httpClient.GetStringAsync(url);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            T result = default(T);

            HttpResponseMessage response = await this._httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsJsonAsync<T>();
            }

            return result;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return await this._httpClient.PostAsync(url, content);
        }
    }
}
