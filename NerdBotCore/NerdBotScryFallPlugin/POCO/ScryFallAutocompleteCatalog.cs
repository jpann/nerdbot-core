using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NerdBotScryFallPlugin.POCO
{
    public class ScryFallAutocompleteCatalog
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("total_values")]
        public int TotalValues { get; set; }

        [JsonProperty("data")]
        public List<string> Data { get; set; }

        public ScryFallAutocompleteCatalog()
        {
            this.Object = string.Empty;
            this.Uri = string.Empty;
            this.TotalValues = 0;
            this.Data = new List<string>();
        }
    }
}
