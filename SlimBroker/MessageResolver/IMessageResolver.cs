using System;
using System.Collections.Generic;

namespace SlimBroker.MessageResolver
{
    public interface IMessageResolver
    {
        IEnumerable<Action<TMessage>> SubscribersFor<TMessage>();
        void RegisterFor<TMessage>(Action<TMessage> subscriber);
    }
}
