using NUnit.Framework;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.Tests
{
    public class CustomChannelBuilderTests
    {
        [Test]
        public void AddNamedChannelForTypedMessage()
        {
            int msgCount = 0;
            const string channelName = "Custom channel";

            IServiceBus bus = Builder.Configure()
                                     .WithChannel(chnCfg =>
                                         {
                                             chnCfg.Channel = new InProcChannel(new NoFilter(), new IsResolver()) { Name = channelName };
                                         })
                                     .Build();
            bus.Subscrive<StringMessage>(message => { msgCount++; });

            bus.Publish(new StringMessage("test message"));
            bus.Publish(1);
            bus.Publish("another one");

            Assert.That(msgCount, Is.EqualTo(1));
        }
    }
}