using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimBroker
{
    public class ServiceBus : IServiceBus, IDispatcher
    {
        private readonly List<IChannel> _channels;

        public ServiceBus()
        {
            _channels = new List<IChannel>();
        }

        public void Publish<TMessage>(TMessage message)
        {
            _channels.Where(ch => ch.Accepts<TMessage>())
                     .ToList()
                     .ForEach(ch => ch.Publish(message));
        }

        public IEnumerable<IChannel> Channels { get { return _channels; }}

        public void Subscribe<TMessage>(Action<TMessage> action)
        {
            _channels.Where(ch => ch.Accepts<TMessage>())
                     .ToList()
                     .ForEach(ch => ch.Register(action));
        }
        
        public void AddChannel(IChannel channel)
        {
            _channels.Add(channel);
        }
        
        public void Dispatch<TMessage>(TMessage message)
        {
            Publish(message);
        }
    }
}