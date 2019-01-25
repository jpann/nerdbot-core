using System.Threading.Tasks;
using NerdBotCommon.Messengers;

namespace NerdBotCommon.Plugin
{
    public interface IMessagePlugin
    {
        string BotName { get; set; }
        IBotServices Services { get; set; }

        string Name { get; }
        string Description { get; }
        string ShortDescription { get; }

        void OnLoad();
        void OnUnload();
        Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}