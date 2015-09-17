using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.SignalR
{
    public class SignalRServerSideChannel : IChannel
    {
        private readonly IDispatcher _dispatcher;
        private readonly IMessageFilter _filter;
        private readonly IMessageResolver _resolver;
        private readonly IDisposable _webApp;
        public string Name { get; set; }

        public SignalRServerSideChannel(IDispatcher dispatcher, IMessageFilter filter, IMessageResolver resolver, string bindingUrl)
        {
            _dispatcher = dispatcher;
            _filter = filter;
            _resolver = resolver;

            _webApp = WebApp.Start<Startup>(bindingUrl);
            GlobalHost.DependencyResolver.Register(typeof (SignalRServerSideChannel), () => this);
        }

        public void Publish<TMessage>(TMessage message)
        {
            Type msgType = typeof(TMessage);
            if (!_filter.Accept<TMessage>())
                throw new InvalidOperationException($"Channel {Name} is not configured to publish {msgType} messages");

            var subscriber = _resolver.SubscribersFor<TMessage>();

            subscriber?.ToList()
                .ForEach(sub => sub.Invoke(message));
        }

        public void Register<TMessage>(Action<TMessage> action)
        {
            Type msgType = typeof (TMessage);
            if (!_filter.Accept<TMessage>())
                throw new InvalidOperationException($"Channel {Name} is not configured to publish {msgType} messages");

            _resolver.RegisterFor(action);
        }
        
        public bool Accepts<TMessage>()
        {
            return _filter.Accept<TMessage>();
        }
        
        public void Dispatch<TMessage>(TMessage message)
        {
            _dispatcher.Dispatch(message);
        }
    }
}