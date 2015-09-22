[![Build status](https://ci.appveyor.com/api/projects/status/k93ijhyaj7qdrw2x?svg=true)](https://ci.appveyor.com/project/fmichellonet/slimbroker)
# SlimBroker
SlimBroker is a simple and modular Service Bus.

### Simplicity

The simpliest configuration, is just no conf!
```csharp
IServiceBus bus = Builder.Build();
```
An in-proc channel is created for you under the hood, which let you connect multiple component throught a publish/subscribe pattern.

Now that you've built a bus, you can easily subscribe to a message type and decide how to handle them.
```csharp
bus.Subscribe<string>(message => { Console.WriteLine(message);});
```

You can easily send message on the bus; fire and forget!
```csharp
bus.Publish<string>("new message");
```
The bus will dispatch this message to all subscribers.

### Channels

Channels are deeply rooted at the heart of SlimBroker. They allow the extensibility of the bus accross multiple devices, network, protocols etc.

#### In Proc channel

An inproc channel runs in the same process as the calling application. 
The typical use case is a Task based process that use the bus to achieve uncoupling between producers and consumers.

#### SignalR channel

Asynchronous communication between multiple devices over http can be achieved with this channel.
To create a bus with a SignalR channel you would use on the server side :
```csharp
string sinkUrl = "http://localhost:8894";
IServiceBus bus = Builder.Configure()
    .WithChannel((dispatcher, chnCfg) =>
    {
        chnCfg.Channel = new SignalRServerSideChannel(dispatcher, new NoFilter(), new IsResolver(), sinkUrl)
        {
            Name = "SignalRChannel1"
        };
    })
    .Build();
```

On the client side instead you can use the following code :
```csharp
string sinkUrl = "http://localhost:8894";
IServiceBus bus =
    Builder.Configure()
        .WithChannel(chnCfg => { chnCfg.Channel = new SignalRClientSideChannel(sinkUrl); })
        .Build();
```

and then simply use Publish and Subscribe bus' methods without paying attention to the underlying declared channels.

### Web ready

You'd like to use SlimBroker.js which implements a SignalR channel when developping in javascript or TypeScript.
```TypeScript
var serverUrl = "http://localhost:8894";
var bus: SlimBroker.ServiceBus;
bus = new SlimBroker.ServiceBus();
bus.addChannel(new SlimBroker.ClientSideSignalRChannel(serverUrl, () => {
                this.bus.register((msg: string) => { Console.Log(msg); }, "System.String");
            }
        );
```
In this example, we've configured a signalR channel and when the channel is connected, we register a callback to handle messages of type System.String

Likewise, you can send a message easily :
```TypeScript
bus.publish(chatMessage, "Sytem.String");
```

## How to start

Retrieve the needed packages via nuget and you're ready 
```sh
Install-Package SlimBroker
```
You might also have a look to the sample folder in the source code.
