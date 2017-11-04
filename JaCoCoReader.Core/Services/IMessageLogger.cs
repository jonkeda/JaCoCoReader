namespace JaCoCoReader.Core.Services
{
    public interface IMessageLogger
    {
        void SendMessage(TestMessageLevel level, string message);
    }
}