using NUnit.Framework;
using SlimBroker.MessageFilter;

namespace SlimBroker.Tests
{
    public class NoFilterTests
    {
        [Test]
        public void NoFilterAcceptAll()
        {
            NoFilter filter = new NoFilter();
            Assert.That(filter.Accept<object>(), Is.True);
        }
    }
}
