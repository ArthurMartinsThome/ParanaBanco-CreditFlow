using CreditFlow.Domain.Event;

namespace CreditFlow.Domain.Event
{
    public record ProposalRejectedEvent(Guid? ClientId, string? Reason) : BaseEvent;
}