using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.SignalR.Tests
{
    public class SignalRClientSideChannelTests
    {
        private readonly IDispatcher _dummyDispatcher;

        public SignalRClientSideChannelTests()
        {
            _dummyDispatcher = new Mock<IDispatcher>().Object;
        }


        [Test]
        public void CanConnectToExistingChannel()
        {
            string sinkUrl = "http://localhost:8888";
            SignalRServerSideChannel srv = new SignalRServerSideChannel(_dummyDispatcher, new NoFilter(),
                new IsResolver(), sinkUrl);

            SignalRClientSideChannel clt = new SignalRClientSideChannel(sinkUrl);
        }

        [Test]
        public void CanRecieveMessageWhenRegistered()
        {
            string sinkUrl = "http://localhost:8889";
            SignalRServerSideChannel srv = new SignalRServerSideChannel(_dummyDispatcher, new NoFilter(),
                new IsResolver(), sinkUrl);

            bool received = false;
            SignalRClientSideChannel clt = new SignalRClientSideChannel(sinkUrl);
            clt.Register<int>(msg =>
            {
                int rcv = msg;
                received = true;
            });

            srv.Publish(10);

            while (!received)
            {
                Task.WaitAny(Task.Delay(1000));
            }
        }

        [Test]
        public void CanRecieveMessageViaBusInstance()
        {
            string sinkUrl = "http://localhost:8890";
            string msg = "hello";
            bool recieved = false;

            Task server = new Task(() =>
            {
                IServiceBus bus =
                    Builder.Configure()
                        .WithChannel(
                            (dispacher, chnCfg) =>
                            {
                                chnCfg.Channel = new SignalRServerSideChannel(dispacher, new NoFilter(),
                                    new IsResolver(), sinkUrl);
                            })
                        .Build();

                while (!recieved)
                    Task.WaitAny(Task.Delay(1000));
            });

            Task producer = new Task(() =>
            {
                IServiceBus bus =
                    Builder.Configure()
                        .WithChannel(chnCfg => { chnCfg.Channel = new SignalRClientSideChannel(sinkUrl); })
                        .Build();

                bus.Publish(msg);

                while (!recieved)
                    Task.WaitAny(Task.Delay(1000));
            });

            Task consumer = new Task(() =>
            {
                IServiceBus bus =
                    Builder.Configure()
                        .WithChannel(chnCfg => { chnCfg.Channel = new SignalRClientSideChannel(sinkUrl); })
                        .Build();

                bus.Subscrive<string>((s =>
                {
                    if (s != msg)
                        Assert.Fail();
                    else
                        recieved = true;

                    while (!recieved)
                        Task.WaitAny(Task.Delay(1000));
                }));
            });

            server.Start();
            consumer.Start();
            producer.Start();

            Task.WaitAll(server, producer, consumer);
        }
    }
}