using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NerdBotUrbanDictPlugin.POCO
{
    public class UrbanDictionaryData
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("list")]
        public List<UrbanDictionaryDefinition> Definitions { get; set; } 
    }

    public class UrbanDictionaryDefinition
    {
        [JsonProperty("defid")]
        public int DefId { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("permalink")]
        public string PermaLink { get; set; }

        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_up")]
        public int ThumbsUp { get; set; }

        [JsonProperty("thumbs_down")]
        public int ThumbsDown { get; set; }
    }
}
