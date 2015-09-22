using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker.SignalR.Tests
{
    public class TasksTests
    {
        
        [Test]
        public void ConsumersWithTasksMustWork()
        {
            string helloMsg = "Hello World";
            bool consumerReady = false;

            string sinkUrl = "http://localhost:8895";

            IServiceBus bus = Builder.Configure()
                .WithChannel(chnCfg => { chnCfg.Channel = new  InProcChannel(new NoFilter(), new IsResolver());}) 
                .WithChannel((dispatcher, chnCfg) =>
                {
                    chnCfg.Channel = new SignalRServerSideChannel(dispatcher, new NoFilter(), new IsResolver(), sinkUrl)
                    {
                        Name = "SignalRChannel1"
                    };
                })
                .Build();


            Task producer = new Task(() =>
            {
                while (!consumerReady)
                {
                    Task.WaitAny(Task.Delay(1000));
                }
                bus.Publish(helloMsg);
            });

            Task consumer = new Task(() =>
            {
                bool shouldContinue = true;
                IServiceBus clientBus = Builder.Configure()
                .WithChannel((dispatcher, chnCfg) =>
                {
                    chnCfg.Channel = new SignalRClientSideChannel(sinkUrl)
                    {
                        Name = "SignalRChannel1"
                    };
                })
                .Build();
                
                clientBus.Subscribe<string>(msg =>
                {
                    if(msg != helloMsg)
                        Assert.Fail();
                    shouldContinue = false;
                });

                consumerReady = true;

                // simulate working load.
                while (shouldContinue)
                {
                    Task.WaitAny(Task.Delay(1000));
                }
            });
            
            consumer.Start();
            // Ensure consumers are started.
            Task.WaitAny(Task.Delay(1000));

            producer.Start();
            
            try
            {
                Task.WaitAll(producer, consumer);
            }
            catch (Exception)
            {
               Assert.Fail();
            }
        }
    }
}