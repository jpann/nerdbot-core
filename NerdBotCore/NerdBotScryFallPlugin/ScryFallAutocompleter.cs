using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotScryFallPlugin.POCO;
using Serilog;

namespace NerdBotScryFallPlugin
{
    public class ScryFallAutocompleter
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;

        private const string cUrl = "https://api.scryfall.com/cards/autocomplete?q={0}";

        public ScryFallAutocompleter(IHttpHandler httpClient, ILogger loggingService)
        {
            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (loggingService == null)
                throw new ArgumentNullException("loggingService");

            this._httpClient = httpClient;
            this._logger = loggingService;
        }

        public async Task<List<string>> GetAutocompleteAsync(string term)
        {
            string url = string.Format(cUrl, term);
            var def = await this._httpClient.GetAsync<ScryFallAutocompleteCatalog>(url);

            if (def == null)
                return null;

            return def.Data;
        }
    }
}
