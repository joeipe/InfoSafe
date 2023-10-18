using Azure.Messaging.ServiceBus;
using InfoSafe.Infra.Bus.Interfaces;

namespace InfoSafe.Infra.Bus
{
    public class AzServiceBus : IBus
    {
        private readonly ServiceBusClient _client;

        public AzServiceBus(string connectionString)
        {
            _client = new ServiceBusClient(connectionString);
        }

        public void Send(string message, string topicName)
        {
            SendTextStringAsync(message, topicName).Wait();
        }

        private async Task SendTextStringAsync(string text, string topicName)
        {
            ServiceBusSender sender = _client.CreateSender(topicName);

            var message = new ServiceBusMessage(text)
            {
                Subject = "InfoSafe.Messages",
                ContentType = "application/json",
            };

            await sender.SendMessageAsync(message);
            await sender.CloseAsync();
        }
    }
}