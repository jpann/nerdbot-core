namespace NerdBotCommon.UrlShorteners
{
    public interface IUrlShortener
    {
        string User { get; }
        string Key { get; }

        string ShortenUrl(string url);
    }
}