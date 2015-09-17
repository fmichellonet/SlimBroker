using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SlimBroker.SignalR
{
    public class SignalRClientSideChannel : IChannel
    {

        private readonly IHubProxy _hub;
        public string Name { get; set; }

        public SignalRClientSideChannel(string bindingUrl)
        {
            HubConnection connection = new HubConnection(bindingUrl);
            _hub = connection.CreateHubProxy("SignalRChannelSink");
            connection.Start().Wait();
        }

        public void Publish<TMessage>(TMessage message)
        {
            _hub.Invoke("Publish", message, message.GetType().AssemblyQualifiedName)
                .Wait();
        }
        
        public void Register<TMessage>(Action<TMessage> action)
        {
            Type msgType = typeof (TMessage);
            
            Task registering = null;
            try
            {
                registering = _hub.Invoke("Register", "Callback", msgType.AssemblyQualifiedName);
                _hub.On<TMessage>("Callback", (msg) =>
                {
                    action.Invoke(msg);
                });
            }
            finally
            {
                registering.Wait();
            }
        }

        public bool Accepts<TMessage>()
        {
            return true;
            //throw new InvalidOperationException();
        }

        public void Dispose()
        {
            //_webApp.Dispose();
        }
    }
}