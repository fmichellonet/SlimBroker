using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Moq;
using NUnit.Framework;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.SignalR.Tests
{
    public class SignalRServerSideChannelTests
    {

        private readonly IDispatcher _dummyDispatcher;

        public SignalRServerSideChannelTests()
        {
            _dummyDispatcher = new Mock<IDispatcher>().Object;
        }

        [Test]
        public void HubMustBeAccessibleWhenInstantiatingChannel()
        {
            string sinkUrl = "http://localhost:8891";
            SignalRServerSideChannel chn = new SignalRServerSideChannel(_dummyDispatcher, new NoFilter(), new IsResolver(), sinkUrl);

            var connection = new HubConnection(sinkUrl);
            var hub = connection.CreateHubProxy("SignalRChannelSink");
            connection.Start().Wait();
            var invokeTsk = hub.Invoke("Register", "callbackName", typeof (int).ToString())
                               .ContinueWith(t => { Assert.Fail(); }, TaskContinuationOptions.NotOnRanToCompletion);

            Task.WaitAny(invokeTsk);
        }

        [Test]
        public void CanConnectDirectlyToHubProxy()
        {
            string sinkUrl = "http://localhost:8892";
            IServiceBus bus = Builder.Configure()
                .WithChannel((dispatcher, chnCfg) =>
                {
                    chnCfg.Channel = new SignalRServerSideChannel(dispatcher, new NoFilter(), new IsResolver(), sinkUrl)
                    {
                        Name = "SignalRChannel1"
                    };
                })
                .Build();

            var connection = new HubConnection(sinkUrl);
            var hub = connection.CreateHubProxy("SignalRChannelSink");
            connection.Start().Wait();
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Connected));
        }

        [Test]
        public void CanRegisterDirectlyToHubProxy()
        {
            string sinkUrl = "http://localhost:8893";
            IServiceBus bus = Builder.Configure()
                .WithChannel((dispatcher, chnCfg) =>
                {
                    chnCfg.Channel = new SignalRServerSideChannel(dispatcher, new NoFilter(), new IsResolver(), sinkUrl)
                    {
                        Name = "SignalRChannel1"
                    };
                })
                .Build();

            var connection = new HubConnection(sinkUrl);
            var hub = connection.CreateHubProxy("SignalRChannelSink");

            connection.Start().Wait();

            hub.Invoke("Register", "callbackName", typeof (int).ToString())
               .Wait();

            Assert.That(connection.State, Is.EqualTo(ConnectionState.Connected));
        }

        [Test]
        public void CanPublishDirectlyToHub()
        {
            string sinkUrl = "http://localhost:8894";
            IServiceBus bus = Builder.Configure()
                .WithChannel((dispatcher, chnCfg) =>
                {
                    chnCfg.Channel = new SignalRServerSideChannel(dispatcher, new NoFilter(), new IsResolver(), sinkUrl)
                    {
                        Name = "SignalRChannel1"
                    };
                })
                .Build();

            var connection = new HubConnection(sinkUrl);
            var hub = connection.CreateHubProxy("SignalRChannelSink");

            int receivedValue = 0;
            int sentValue = 48;
            bool callbackCalled = false;

            
            // Action that will be called when a msg is sent.
            hub.On<int>("callbackName", (msg) =>
            {
                receivedValue = msg;
                callbackCalled = true;
            });

            connection.Start().Wait();

            hub.Invoke("Register", "callbackName", typeof (int).ToString())
               .Wait();
            
            bus.Publish(sentValue);

            while (!callbackCalled)
                Task.WaitAny(Task.Delay(100));

            Assert.That(sentValue, Is.EqualTo(receivedValue));
        }
    }
}