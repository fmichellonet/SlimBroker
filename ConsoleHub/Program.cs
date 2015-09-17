using System;
using System.Threading.Tasks;
using Chat.Model;
using SlimBroker;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;
using SlimBroker.SignalR;

namespace Samples.Chat.HubServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string sinkUrl = "http://localhost:9000";
            bool recieved = false;
            bool started = false;

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
                started = true;
                while (!recieved)
                    Task.WaitAny(Task.Delay(10000));
            });
            server.Start();
            
            
            Console.WriteLine($"Server up and listening on {sinkUrl}");
            Console.ReadLine();
        }
    }
}