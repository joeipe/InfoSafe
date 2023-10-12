using InfoSafe.Infra.Bus;
using InfoSafe.Write.Data.Events;
using SharedKernel.Interfaces;

namespace InfoSafe.Write.Data.EventDispatchers
{
    public class EventDispatcher
    {
        private readonly MessageBus _messageBus;

        public EventDispatcher(MessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void Dispatch(IEnumerable<IDomainEvent> events)
        {
            foreach (IDomainEvent ev in events)
            {
                Dispatch(ev);
            }
        }

        public void Dispatch(IDomainEvent ev)
        {
            switch (ev)
            {
                case ContactSavedEvent contactSavedEvent:
                    _messageBus.SendMessage(contactSavedEvent, "ContactSavedMessageTopic");
                    break;

                default:
                    throw new Exception($"Unknown event type: '{ev.GetType()}'");
            }
        }
    }
}