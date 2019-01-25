using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using Newtonsoft.Json.Linq;
using Serilog;

namespace NerdBotGiphyPlugin
{
    public class GiphyFetcher
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;
        private string _url = "http://api.giphy.com/v1/gifs/translate?s={0}&api_key=dc6zaTOxFJmzC";

        public GiphyFetcher(IHttpHandler httpClient, ILogger logger)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            if (logger == null)
                throw new ArgumentException("logger");

            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<string> GetGifAsync(string keyword)
        {
            try
            {
                keyword = Uri.EscapeDataString(keyword);

                string latestJson = await this._httpClient.GetStringAsync(string.Format(this._url, keyword));

                if (string.IsNullOrEmpty(latestJson))
                    return null;

                JObject giphy = JObject.Parse(latestJson);

                string url = (string)giphy["data"]["images"]["original"]["url"];

                return url;
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"ERROR getting giphy gif for '{keyword}': {er.Message}");

                throw;
            }
        }
    }
}
