using System;
using System.Collections.Generic;

namespace SlimBroker
{
    public interface IServiceBus
    {
        void Subscrive<TMessage>(Action<TMessage> action);
        void Publish<TMessage>(TMessage message);
        IEnumerable<IChannel> Channels { get;}
    }
}