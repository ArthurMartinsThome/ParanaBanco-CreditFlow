using CreditFlow.Domain.Event;

namespace CreditFlow.Integration.RabbitMq.Interface
{
    public interface IEventHandler<in TEvent> where TEvent : BaseEvent
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}