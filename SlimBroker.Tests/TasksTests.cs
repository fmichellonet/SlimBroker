using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SlimBroker.Tests
{
    public class TasksTests
    {
        
        [Test]
        public void ConsumersWithTasksMustWork()
        {
            IServiceBus bus = Builder.Build();
            int sent = 0;
            int received1 = 0;
            int received2 = 0;

            Task producer = new Task(() =>
            {
                bus.Publish(new StringMessage("First"));
                sent++;
                while (new Random().Next(10)%3 != 0)
                {
                    bus.Publish(new StringMessage("CONTINUE"));
                    sent++;
                }

                bus.Publish(new StringMessage("TERMINATE"));
                sent++;

            });

            Task consumer1 = new Task(() =>
            {
                bool shouldContinue = true;

                bus.Subscrive<StringMessage>(message =>
                {
                    received1++;
                    if (message.Message == "TERMINATE")
                        shouldContinue = false;
                });

                // simulate working load.
                while (shouldContinue)
                {
                    Task.WaitAny(Task.Delay(100));
                }
            });

            Task consumer2 = new Task(() =>
            {
                bool shouldContinue = true;

                bus.Subscrive<StringMessage>(message =>
                {
                    received2++;
                    if (message.Message == "TERMINATE")
                        shouldContinue = false;
                });

                // simulate working load.
                while (shouldContinue)
                {
                    Task.WaitAny(Task.Delay(100));
                }
            });

            consumer1.Start();
            consumer2.Start();
            // Ensure consumers are started.
            Task.WaitAny(Task.Delay(1000));

            producer.Start();
            
            try
            {
                Task.WaitAll(producer, consumer1, consumer2);
            }
            catch (Exception)
            {
               Assert.Fail();
            }
            Assert.That(sent, Is.EqualTo(received1));
            Assert.That(sent, Is.EqualTo(received2));
        }
    }
}