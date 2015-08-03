namespace SlimBroker
{
    public interface IDispatcher
    {
        void Dispatch<TMessage>(TMessage message);
    }
}