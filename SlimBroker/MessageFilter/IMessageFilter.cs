namespace SlimBroker.MessageFilter
{
    public interface IMessageFilter
    {
        bool Accept<TMessage>();
    }
}