using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotUrbanDictPlugin.POCO;
using Newtonsoft.Json;
using Serilog;

namespace NerdBotUrbanDictPlugin
{
    public class UrbanDictionaryFetcher
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;
        private readonly string _url;

        public UrbanDictionaryFetcher(string url, IHttpHandler httpClient, ILogger logger)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (httpClient == null)
                throw new ArgumentException("httpClient");

            if (logger == null)
                throw new ArgumentException("logger");

            this._url = url;
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<UrbanDictionaryData> GetDefinition(string word)
        {
            try
            {
                string url = string.Format(this._url, word);

                var def = await this._httpClient.GetAsync<UrbanDictionaryData>(url);

                if (def == null)
                    return null;

                return def;
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"ERROR getting urban dictionary definition for '{word}'");

                throw;
            }
        }
    }
}
