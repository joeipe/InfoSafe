using InfoSafe.Infra.Bus.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace InfoSafe.Infra.Bus
{
    public class RmqServiceBus : IBus
    {
        private readonly string _connectionString;

        public RmqServiceBus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Send(string message, string queueName)
        {
            var factory = new ConnectionFactory() { HostName = _connectionString };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, false, false, false, null);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", queueName, null, body);
            }
        }
    }
}