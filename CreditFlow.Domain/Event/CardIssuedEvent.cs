using CreditFlow.Domain.Event;

namespace CreditFlow.Domain.Event
{
    public record CardIssuedEvent(Guid? ClientId, string? PartialCardNumber, string? FinalStatus, decimal? ApprovedLimit) : BaseEvent;
}