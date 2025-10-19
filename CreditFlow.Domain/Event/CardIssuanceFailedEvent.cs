using CreditFlow.Domain.Event;

namespace CreditFlow.Domain.Event
{
    public record CardIssuanceFailedEvent(Guid? ClientId, string? FailureReason) : BaseEvent;
}