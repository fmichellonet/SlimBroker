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
bus.Subscrive<string>(message => { Console.WriteLine(message);});
```

You can easily send message on the bus; Fire and forget.
```csharp
bus.Publish<string>("new message");
```

### Channels

