namespace SlimBroker.SignalR
{
    public class RoutingConfig
    {
        public RoutingConfig(string callbackMethodName, string clientConnectionId)
        {
            CallbackMethodName = callbackMethodName;
            ClientConnectionId = clientConnectionId;
        }

        public string CallbackMethodName { get; }

        public string ClientConnectionId { get; }
    }
}