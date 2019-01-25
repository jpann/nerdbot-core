using System;
using System.Linq;
using System.Threading.Tasks;
using NerdBotCommon;
using NerdBotCommon.Extensions;
using Newtonsoft.Json.Linq;
using Serilog;

namespace NerdBotRoastMePlugin
{
    public class RedditTopCommentFetcher
    {
        private const string cUrl = "https://www.reddit.com/{0}/random/.json?sort=%27top%27&limit=1";

        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;

        public RedditTopCommentFetcher(IHttpHandler httpClient, ILogger logger)
        {
            if (httpClient == null)
                throw new ArgumentException("httpClient");

            if (logger == null)
                throw new ArgumentException("logger");

            this._httpClient = httpClient;
            this._logger = logger;
        }

        public async Task<string> GetTopCommentFromSubreddit(string subreddit)
        {
            string text = null;

            try
            {
                string url = string.Format(cUrl, subreddit);
                var response = this._httpClient.Get(url);

                if (response.IsSuccessStatusCode)
                {

                    string result = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(result))
                    {
                        var json = JArray.Parse(result);

                        if (json != null)
                        {
                            text = json.Last()["data"]["children"][0]["data"]["body"].ToString();
                        }
                    }
                }             

                return text;
            }
            catch (Exception er)
            {
                this._logger.Here().Error(er, "ERROR getting data");

                return null;
            }
        }
    }
}
