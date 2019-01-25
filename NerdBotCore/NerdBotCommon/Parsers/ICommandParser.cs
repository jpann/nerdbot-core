namespace NerdBotCommon.Parsers
{
    public interface ICommandParser
    {
        Command Parse(string text);
    }
}