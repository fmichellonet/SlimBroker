using System;
using System.Linq;
using NUnit.Framework;
using SlimBroker.MessageResolver;

namespace SlimBroker.Tests
{
    public class IsResolverTests
    {
        
        [Test]
        public void ASubscriberIsRetrieved()
        {
            IsResolver resolver = new IsResolver();
            resolver.RegisterFor<int>(i => { });
            Assert.That(resolver.SubscribersFor<int>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void MultipleSubcriberAreCalledFor1MessageType()
        {
            IsResolver resolver = new IsResolver();
            Action<int> sub1 = i => {  };
            Action<int> sub2 = i => {  };
            resolver.RegisterFor(sub1);
            resolver.RegisterFor(sub2);
            Assert.That(resolver.SubscribersFor<int>().ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void DeadSubsribersAreKindlySkipped()
        {
            IsResolver resolver = new IsResolver();
            Action<int> sub = i => { };
            resolver.RegisterFor(sub);
            sub = null;
            Assert.That(resolver.SubscribersFor<int>().ToList(), Has.Count.EqualTo(1));
        }
    }
}
