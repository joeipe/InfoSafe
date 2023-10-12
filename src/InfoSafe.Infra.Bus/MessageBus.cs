using InfoSafe.Infra.Bus.Interfaces;
using SharedKernel.Extensions;
using SharedKernel.Interfaces;

namespace InfoSafe.Infra.Bus
{
    public class MessageBus
    {
        private readonly IBus _bus;

        public MessageBus(IBus bus)
        {
            _bus = bus;
        }

        public void SendMessage(IDomainEvent ev, string queueName)
        {
            _bus.Send(ev.OutputJson(), queueName);
        }
    }
}