using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotScryFallPlugin.POCO;
using Newtonsoft.Json;
using Serilog;

namespace NerdBotScryFallPlugin
{
    public class ScryFallFetcher
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;

        private const string cApiUrl = "https://api.scryfall.com";

        public ScryFallFetcher(IHttpHandler httpClient, ILogger logger)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            if (logger == null)
                throw new ArgumentException("logger");

            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<ScryFallCard> GetCard(int multiverseId)
        {
            try
            {
                string cardApi = "/cards/multiverse/{0}";

                string url = string.Format(cApiUrl + cardApi, multiverseId);

                this._logger.Verbose($"Getting card using url '{url}'...");

                var def = await this._httpClient.GetAsync<ScryFallCard>(url);

                if (def == null)
                    return null;

                return null;
            }
            catch (Exception er)
            {
                string msg = $"ERROR getting ScryFall Card for '{multiverseId}': {er.Message}";

                this._logger.Error(er, msg);

                throw;
            }
        }

        public async Task<ScryFallCard> GetCard(string name)
        {
            bool useFuzzySearch = false;
            if (name.Contains('%') || name.Contains('*'))
            {
                useFuzzySearch = true;
                name = name.Replace("%", "").Replace("*", "");
            }

            try
            {
                string cardApi = "/cards/named?{0}={1}";

                string encodedName = Uri.EscapeDataString(name);

                string parameter = "exact";

                if (useFuzzySearch)
                {
                    parameter = "fuzzy";
                }
                else
                {
                    parameter = "exact";
                }

                string url = string.Format(cApiUrl + cardApi, parameter, encodedName);

                this._logger.Verbose($"Getting card using url '{url}'...");

                var def = await this._httpClient.GetAsync<ScryFallCard>(url);

                if (def == null)
                    return null;

                return def;
            }
            catch (Exception er)
            {
                string msg = $"ERROR getting ScryFall Card for '{name}': {er.Message}";

                this._logger.Error(er, msg);

                throw;
            }
        }

        public async Task<ScryFallCard> GetCard(string name, string setCode)
        {
            bool useFuzzySearch = false;
            if (name.Contains('%') || name.Contains('*'))
            {
                useFuzzySearch = true;
                name = name.Replace("%", "").Replace("*", "");
            }

            try
            {
                string cardApi = "/cards/named?{0}={1}&set={2}";

                string encodedName = Uri.EscapeDataString(name);
                string encodedSetCode = Uri.EscapeDataString(setCode);

                string parameter = "exact";

                if (useFuzzySearch)
                {
                    parameter = "fuzzy";
                }
                else
                {
                    parameter = "exact";
                }

                string url = string.Format(cApiUrl + cardApi, parameter, encodedName, encodedSetCode);

                this._logger.Verbose($"Getting card using url '{url}'...");

                var def = await this._httpClient.GetAsync<ScryFallCard>(url);

                if (def == null)
                    return null;

                return def;
            }
            catch (Exception er)
            {
                string msg = $"ERROR getting ScryFall Card for '{name}' in set '{setCode}': {er.Message}";

                this._logger.Error(er, msg);

                throw;
            }
        }
    }
}
