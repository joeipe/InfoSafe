using InfoSafe.Infra.Bus.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace InfoSafe.Infra.Bus
{
    public class RmqServiceBus : IBus
    {
        private readonly ConnectionFactory _factory;

        public RmqServiceBus(string connectionString)
        {
            _factory = new ConnectionFactory() { HostName = connectionString };
        }

        public void Send(string message, string topicName)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(topicName, false, false, false, null);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", topicName, null, body);
            }
        }
    }
}