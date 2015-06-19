namespace SlimBroker.MessageFilter
{
    public class NoFilter : IMessageFilter
    {

        public bool Accept<TMessage>()
        {
            return true;
        }
    }
}