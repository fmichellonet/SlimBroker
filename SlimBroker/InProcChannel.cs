using System;
using System.Linq;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker
{
    public class InProcChannel : IChannel
    {
        private readonly IMessageFilter _filter;
        private readonly IMessageResolver _resolver;

        public InProcChannel(IMessageFilter filter, IMessageResolver resolver)
        {
            _filter = filter;
            _resolver = resolver;
        }

        public string Name { get; set; }

        public void Publish<TMessage>(TMessage message)
        {
            Type msgType = typeof(TMessage);
            if(!_filter.Accept<TMessage>())
                throw new InvalidOperationException(string.Format("Channel {0} is not configured to publish {1} messages", Name, msgType));

            var subscriber = _resolver.SubscribersFor<TMessage>();
            
            subscriber?.ToList()
                .ForEach(sub => sub.Invoke(message));
        }

        public void Register<TMessage>(Action<TMessage> action)
        {
            Type msgType = typeof(TMessage);
            if (!_filter.Accept<TMessage>())
                throw new InvalidOperationException(string.Format("Channel {0} is not configured to publish {1} messages", Name, msgType));

            _resolver.RegisterFor(action);
        }

        public bool Accepts<TMessage>()
        {
            return _filter.Accept<TMessage>();
        }
    }
}