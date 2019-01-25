using System.Net.Http;
using System.Threading.Tasks;

namespace NerdBotCommon
{
    public interface IHttpHandler
    {
        HttpResponseMessage Get(string url);
        string GetString(string url);
        HttpResponseMessage Post(string url, HttpContent content);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<string> GetStringAsync(string url);
        Task<T> GetAsync<T>(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
    }
}