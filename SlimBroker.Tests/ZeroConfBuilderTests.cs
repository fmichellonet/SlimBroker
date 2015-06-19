using System;
using NUnit.Framework;

namespace SlimBroker.Tests
{
    public class ZeroConfBuilderTests
    {
        [Test]
        public void CanBuild()
        {
            IServiceBus bus = Builder.Build();
            Assert.That(bus, Is.Not.Null);
        }

        [Test]
        public void CanPublish()
        {
            IServiceBus bus = Builder.Build();
            bus.Publish(new {msg = "new message"});
        }

        [Test]
        public void CanSubscribeToAnything()
        {
            IServiceBus bus = Builder.Build();
            bus.Subscrive<object>(message => {});
        }

        [Test]
        public void CanSubscribeToTypedMessage()
        {
            IServiceBus bus = Builder.Build();
            bus.Subscrive<StringMessage>(message => {});
        }

        [Test]
        public void PublishedMessageIsDelivered()
        {
            bool called = false;
            StringMessage strMessage = new StringMessage("This is a typed message");
            Action<StringMessage> subscriber = message => { called = true; };
            
            IServiceBus bus = Builder.Build();
            bus.Subscrive(subscriber);
            bus.Publish(strMessage);

            Assert.That(called, Is.True);
        }
    }
}