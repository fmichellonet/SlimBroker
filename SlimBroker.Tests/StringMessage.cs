namespace SlimBroker.Tests
{
    public class StringMessage
    {
        private readonly string _message;

        public StringMessage(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}