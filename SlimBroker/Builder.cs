using System;
using SlimBroker.MessageFilter;
using SlimBroker.MessageResolver;

namespace SlimBroker
{
    public class Builder : IConfiguringBuilder
    {
        private readonly ServiceBus _buildingBus;

        private Builder()
        {
            _buildingBus = new ServiceBus();
        }

        // build bus with no conf.
        public static IServiceBus Build()
        {
            ServiceBus bus = new ServiceBus();
            bus.AddChannel(new InProcChannel(new NoFilter(), new IsResolver()) { Name = "Default channel" });
            return bus;
        }

        public static IConfiguringBuilder Configure()
        {
            return new Builder();
        }

        // return the bus that has been progressively built 
        IServiceBus IConfiguringBuilder.Build()
        {
            return _buildingBus;
        }

        // Configure a new channel
        public IConfiguringBuilder WithChannel(Action<ChannelConfig> action)
        {
            ChannelConfig cfg = new ChannelConfig();
            action.Invoke(cfg);
            _buildingBus.AddChannel(cfg.Channel);
            return this;
        }
    }

    public interface IConfiguringBuilder
    {
        IServiceBus Build();
        IConfiguringBuilder WithChannel(Action<ChannelConfig> action);
    }

    public class ChannelConfig
    {
        public string Name { get; set; }
        public IChannel Channel { get; set; }
        internal Type[] MsgTypes = new[]{typeof(object)};

        public ChannelConfig()
        {
            Channel = new InProcChannel(new NoFilter(), new IsResolver()) {Name = Guid.NewGuid().ToString()};
        }

        public void ForMessages(Type[] types)
        {
            MsgTypes = types;
        }
    }
}