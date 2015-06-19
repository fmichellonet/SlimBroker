using System.Linq;
using NUnit.Framework;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.Tests
{
    public class ChannelTests
    {
        [Test]
        public void AddNamedChannel()
        {
            const string channelName = "Custom channel";

            IServiceBus bus = Builder.Configure().WithChannel(chnCfg =>
                {
                    chnCfg.Channel = new InProcChannel(new NoFilter(), new IsResolver()) { Name = channelName };
                })
                                     .Build();

            Assert.That(bus.Channels.Any(ch => ch.Name == channelName));
        }
    }
}