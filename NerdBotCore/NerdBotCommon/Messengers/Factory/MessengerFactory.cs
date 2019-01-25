using System;
using System.Collections.Generic;
using System.Text;
using Nancy.TinyIoc;

namespace NerdBotCommon.Messengers.Factory
{
    public class MessengerFactory : IMessengerFactory
    {
        private readonly TinyIoCContainer mContainer;

        public MessengerFactory(TinyIoCContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.mContainer = container;
        }

        public IMessenger Create()
        {
            var messenger = this.mContainer.Resolve<IMessenger>();

            return messenger;
        }

        public IMessenger Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            var messenger = this.mContainer.Resolve<IMessenger>(name);

            return messenger;
        }
    }
}
