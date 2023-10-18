using InfoSafe.Infra.Bus.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace InfoSafe.Infra.Bus.ExtraForLearning
{
    public class RmqFanOutServiceBus : IBus
    {
        private readonly ConnectionFactory _factory;

        public RmqFanOutServiceBus(string connectionString)
        {
            _factory = new ConnectionFactory() { HostName = connectionString };
        }

        public void Send(string message, string topicName)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(topicName, ExchangeType.Fanout);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(topicName, string.Empty, null, body);
            }
        }
    }
}