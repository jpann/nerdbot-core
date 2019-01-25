using NerdBotCommon.Parsers;
using NerdBotCommon.POCOS;
using NerdBotCommon.UrlShorteners;
using Serilog;

namespace NerdBotCommon.Plugin
{
    public interface IBotServices
    {
        ICommandParser CommandParser { get; }
        IHttpHandler HttpClient { get; }
        IUrlShortener UrlShortener { get; }
        BotConfiguration BotConfig { get; }
        ILogger Logger { get; }
    }
}