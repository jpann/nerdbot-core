using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace NerdBotCommon.Messengers.GroupMe
{
    public class GroupMeMessageValidator : AbstractValidator<GroupMeMessage>
    {
        public GroupMeMessageValidator()
        {
            RuleFor(msg => msg.group_id).NotEmpty();
            RuleFor(msg => msg.id).NotEmpty();
            RuleFor(msg => msg.name).NotEmpty();
            RuleFor(msg => msg.source_guid).NotEmpty();
            // Removed because image messages do not contain a value in 'text'
            //RuleFor(msg => msg.text).NotEmpty();
            RuleFor(msg => msg.user_id).NotEmpty();
            RuleFor(msg => msg.created_at).NotEmpty();
        }
    }

    public class GroupMeMessage : IMessage
    {
        public string name { get; set; }
        public string user_id { get; set; }
        public string text { get; set; }
        public string group_id { get; set; }
        public string id { get; set; }
        public string source_guid { get; set; }
        public double created_at { get; set; }
        public string avatar_url { get; set; }
        public bool system { get; set; }

        public DateTime created_date
        {
            get
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime created_date = origin.AddSeconds(this.created_at);

                return created_date;
            }
        }
    }
}
