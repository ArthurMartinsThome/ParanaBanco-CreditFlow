using MediatR;

namespace CreditFlow.Domain.Event
{
    public abstract record BaseEvent : INotification
    {
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; protected set; }

        protected BaseEvent()
        {
            Timestamp = DateTime.Now;
        }
    }
}