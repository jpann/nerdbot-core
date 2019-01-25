using System;
using System.Collections.Generic;
using System.Text;
using Nancy.Helpers;
using Serilog;

namespace NerdBotCommon.UrlShorteners
{
    public class BitlyShorten : IUrlShortener
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;
        private readonly string _user;
        private readonly string _key;

        public string User
        {
            get { return this._user; }
        }

        public string Key
        {
            get { return this._key;  }
        }

        public BitlyShorten(string user, string key, IHttpHandler httpClient, ILogger logger)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException("user");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this._user = user;
            this._key = key;
            this._httpClient = httpClient;
            this._logger = logger;
        }

        public string ShortenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            try
            {
                string uri = $"http://api.bit.ly/shorten?version=2.0.1&format=txt&longUrl={HttpUtility.UrlEncode(url)}&login={HttpUtility.UrlEncode(this._user)}&apiKey={HttpUtility.UrlEncode(this._key)}";

                string shortened = this._httpClient.GetString(uri);

                return shortened;
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"Error shortening url '{url}'.");
                return url;
            }
        }
    }
}
