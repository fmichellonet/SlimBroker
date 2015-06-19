using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SlimBroker.MessageResolver
{
    public class IsResolver : IMessageResolver
    {

        private readonly ConcurrentDictionary<Type, IList<WeakReference>> _subscribers;

        public IsResolver()
        {
            _subscribers = new ConcurrentDictionary<Type, IList<WeakReference>>();
        }

        public IEnumerable<Action<TMessage>> SubscribersFor<TMessage>()
        {
            Type msgType = typeof(TMessage);
            if (!_subscribers.ContainsKey(msgType))
                return null;

            return _subscribers[msgType].Where(sub => sub.IsAlive)
                                        .Select(s => s.Target)
                                        .Cast<Action<TMessage>>();
        }

        public void RegisterFor<TMessage>(Action<TMessage> subscriber)
        {
            Type msgType = typeof(TMessage);

            IList<WeakReference> subs = _subscribers.ContainsKey(msgType)
                                            ? _subscribers[msgType]
                                            : new List<WeakReference>();
            
            subs.Add(new WeakReference(subscriber));
            _subscribers[msgType] = subs;       
        }
    }
}