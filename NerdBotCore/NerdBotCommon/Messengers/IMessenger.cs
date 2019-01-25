using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NerdBotCommon.Messengers
{
    public interface IMessenger
    {
        string BotId { get; }
        string BotName { get; }
        string[] IgnoreNames { get; }

        Task<bool>SendMessage(string message);
        Task<bool> SendMessageWithMention(string message, string mentionId, int start, int end);
    }
}
