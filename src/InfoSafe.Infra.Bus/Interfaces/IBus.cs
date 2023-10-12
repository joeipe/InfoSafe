namespace InfoSafe.Infra.Bus.Interfaces
{
    public interface IBus
    {
        void Send(string message, string queueName);
    }
}