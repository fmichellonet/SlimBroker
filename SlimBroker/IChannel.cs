using System;

namespace SlimBroker
{
    public interface IChannel
    {
        string Name { get; set; }
        void Publish<TMessage>(TMessage msg);
        void Register<TMessage>(Action<TMessage> action);
        bool Accepts<TMessage>();
    }
}