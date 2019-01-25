using System;
using System.Collections.Generic;
using System.Text;

namespace NerdBotCommon.Messengers
{
    public interface IMessage
    {
        string name { get; set; }
        string user_id { get; set; }
        string text { get; set; }
        DateTime created_date { get; }
    }
}
