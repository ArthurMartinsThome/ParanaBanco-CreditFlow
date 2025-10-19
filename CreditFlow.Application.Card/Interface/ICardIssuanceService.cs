using CreditFlow.Domain.Event;

namespace CreditFlow.Application.Card.Interface
{
    public interface ICardIssuanceService
    {
        Task ProcessIssuanceAsync(ProposalApprovedEvent acceptedEvent);
    }
}