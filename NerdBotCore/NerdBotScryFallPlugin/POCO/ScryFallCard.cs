using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NerdBotScryFallPlugin.POCO
{
    public class ScryFallCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("multiverse_ids")]
        public List<int> MultiverseIds { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("scryfall_uri")]
        public string ScryFallUri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image_uris")]
        public ScryFall_ImgUri Image_Uris { get; set; }

        //[JsonProperty("usd")]
        //public string PriceUsd { get; set; }

        [JsonProperty("set")]
        public string SetCode { get; set; }

        [JsonProperty("set_name")]
        public string SetName { get; set; }

        [JsonProperty("related_uris")]
        public ScryFall_RelatedUri Related_Uris { get; set; }

        [JsonProperty("purchase_uris")]
        public ScryFall_PurchaseUri Purchase_Uris { get; set; }

        [JsonProperty("card_faces")]
        public List<ScryFall_CardFace> Card_Faces { get; set; }

        [JsonProperty("prices")]
        public ScryFall_Prices Prices { get; set; }
    }

    public class ScryFall_ImgUri
    {
        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("normal")]
        public string Normal { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("png")]
        public string Png { get; set; }

        [JsonProperty("art_crop")]
        public string ArtCrop { get; set; }

        [JsonProperty("border_crop")]
        public string BorderCrop { get; set; }
    }

    public class ScryFall_RelatedUri
    {
        [JsonProperty("gatherer")]
        public string Gatherer { get; set; }

        [JsonProperty("tcgplayer_decks")]
        public string TcgPlayerDecks { get; set; }

        [JsonProperty("edhrec")]
        public string Edhrec { get; set; }

        [JsonProperty("mtgtop8")]
        public string MtgTop8 { get; set; }
    }

    public class ScryFall_PurchaseUri
    {
        [JsonProperty("amazon")]
        public string Amazon { get; set; }

        [JsonProperty("ebay")]
        public string EBay { get; set; }

        [JsonProperty("tcgplayer")]
        public string TcgPlayer { get; set; }

        [JsonProperty("magiccardmarket")]
        public string MagicCardMarket { get; set; }

        [JsonProperty("cardhoarder")]
        public string CardHoarder { get; set; }

        [JsonProperty("card_kingdom")]
        public string CardKingdom { get; set; }

        [JsonProperty("mtgo_traders")]
        public string MtgoTraders { get; set; }

        [JsonProperty("coolstuffinc")]
        public string CoolStuffInc { get; set; }
    }

    public class ScryFall_CardFace
    {
        [JsonProperty("name")]
        public string Name { get;set; }

        [JsonProperty("image_uris")]
        public ScryFall_ImgUri Image_Uris { get; set; }
    }

    public class ScryFall_Prices
    {
        [JsonProperty("usd")]
        public string USD { get; set; }

        [JsonProperty("usd_foil")]
        public string USDFoil { get; set; }

        [JsonProperty("eur")]
        public string Euro { get; set; }

    }
}
