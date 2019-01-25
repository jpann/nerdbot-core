using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;

namespace NerdBotCommon.Plugin
{
    public interface IPlugin
    {
        IBotServices Services { get; set; }

        string Name { get; }
        string Description { get; }
        string ShortDescription { get; }
        string Command { get; }
        string HelpCommand { get; }
        string HelpDescription { get; }

        void OnLoad();
        void OnUnload();
        Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger);
    }
}