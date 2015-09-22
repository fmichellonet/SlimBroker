using System;
using System.Threading.Tasks;
using Chat.Model;
using SlimBroker;
using SlimBroker.SignalR;

namespace ConsoleChatClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            ChatMessage msg2 = new ChatMessage();
            Type tt = msg2.GetType();
            
            string sinkUrl = "http://localhost:9000";

            const string QUIT_CMD = ">!Quit";
            bool running = true;

            Task client = new Task(() =>
            {
                IServiceBus bus =
                    Builder.Configure()
                        .WithChannel(
                            (dispacher, chnCfg) => { chnCfg.Channel = new SignalRClientSideChannel(sinkUrl); })
                        .Build();
                Console.WriteLine($"Client connected to {sinkUrl}");
                bus.Subscribe<ChatMessage>(msg => { Console.WriteLine($"[{msg.Sender}] : {msg.Message}"); });

                while (running)
                {
                    Task.WaitAny(Task.Delay(1000));

                    var cmd = Console.ReadLine();
                    if (cmd == QUIT_CMD)
                        running = false;

                    bus.Publish(new ChatMessage {Message = cmd, Sender = "Console Guy"} );
                }
            });

            client.Start();

            while (running)
            {
                Task.WaitAny(Task.Delay(10000));
            }
        }
    }
}
