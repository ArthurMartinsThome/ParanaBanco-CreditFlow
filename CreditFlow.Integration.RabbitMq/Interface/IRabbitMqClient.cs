using CreditFlow.Domain.Event;

namespace CreditFlow.Integration.RabbitMq.Interface
{
    public interface IRabbitMqClient
    {
        public void Publish<T>(T @event) where T : BaseEvent;

        public void Subscribe<T, TH>()
            where T : BaseEvent
            where TH : IEventHandler<T>;

        public void StartConsumer();
    }
}