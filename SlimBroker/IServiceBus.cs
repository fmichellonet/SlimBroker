using System;
using System.Collections.Generic;

namespace SlimBroker
{
    public interface IServiceBus
    {
        void Subscribe<TMessage>(Action<TMessage> action);
        void Publish<TMessage>(TMessage message);
        IEnumerable<IChannel> Channels { get;}
    }
}