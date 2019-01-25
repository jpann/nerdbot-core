using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Plugin;

namespace NerdBotRoastMePlugin
{
    public class NerdBotRoastMePlugin : MessagePluginBase
    {
        private const string cSubReddit = "r/RoastMe";
        private const int cReplyChance = 5;

        private RedditTopCommentFetcher _fetcher;
        private Random _random;

        public override string Name
        {
            get { return "Reddit Top Reply"; }
        }

        public override string Description
        {
            get { return "Randomly reply to message with top comment in a given subreddit."; }
        }

        public override string ShortDescription
        {
            get { return "Randomly reply to message with top comment in a given subreddit."; }
        }

        public NerdBotRoastMePlugin(IBotServices services) : base(services)
        {
        }

        public override void OnLoad()
        {
            this._fetcher = new RedditTopCommentFetcher(this.Services.HttpClient, this.Logger);
            this._random = new Random();
        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnMessage(IMessage message, IMessenger messenger)
        {
            // Exit if message was sent by the bot
            if (message.name.ToLower() == this.BotName.ToLower())
                return false;

            // If a message contains 'roast me', get a random r/roastme top comment
            if (message.text.ToLower().Contains("roast me"))
            {
                string reply = await this.GetRoast();

                if (!string.IsNullOrEmpty(reply))
                {
                    string name = "@" + message.name;

                    string msg = string.Format("{0} {1}", name, reply);

                    int start = 0;
                    int end = msg.IndexOf(name) + name.Length;

                    messenger.SendMessageWithMention(msg, (string)message.user_id, start, end);
                }
            }
            else
            {
                if (this._random.Next(1, 101) <= cReplyChance)
                {
                    string reply = await this.GetRoast();

                    if (!string.IsNullOrEmpty(reply))
                    {
                        string name = "@" + message.name;

                        string msg = string.Format("{0} {1}", name, reply);

                        int start = 0;
                        int end = msg.IndexOf(name) + name.Length;

                        messenger.SendMessageWithMention(msg, (string)message.user_id, start, end);
                    }
                }
            }

            return false;
        }

        private async Task<string> GetRoast()
        {
            bool goodResponse = false;

            string reply = null;

            do
            {
                reply = await this._fetcher.GetTopCommentFromSubreddit(cSubReddit);

                // This is lame, but check if the reply conatins url syntax and contains text
                // If it doesn't, its a good response.
                if (!reply.Contains("[") && !string.IsNullOrEmpty(reply))
                {
                    goodResponse = true;
                }
            } while (goodResponse == false);

            return reply;
        }
    }
}
