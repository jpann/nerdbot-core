using System.Collections.Generic;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;

namespace NerdBotHost.PluginManager
{
    public interface IPluginManager
    {
        string BotName { get; set; }
        string PluginDirectory { get; set; }
        List<IPlugin> Plugins { get; }
        List<IMessagePlugin> MessagePlugins { get; }

        void LoadPlugins();
        void UnloadPlugins();

        Task<bool> HandleCommandAsync(Command command, IMessage message, IMessenger messenger);
        Task<bool> HandledHelpCommandAsync(Command command, IMessenger messenger);
    }
}