using CreditFlow.Domain.Event;

namespace CreditFlow.Domain.Event
{
    public record ProposalApprovedEvent(Guid? ClientId, decimal? ApprovedLimit) : BaseEvent;
}