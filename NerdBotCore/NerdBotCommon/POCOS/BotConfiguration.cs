using System;
using System.Collections.Generic;
using System.Text;

namespace NerdBotCommon.POCOS
{
    public class BotConfiguration
    {
        public string BotName { get; set; }
        public List<BotRoute> Routes { get; set; }
        public Dictionary<string, string> EnvironmentVariables { get; set; }

        public BotConfiguration()
        {
            this.Routes = new List<BotRoute>();
            this.EnvironmentVariables = new Dictionary<string, string>();
        }
    }
}
