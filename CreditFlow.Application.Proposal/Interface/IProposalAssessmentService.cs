using CreditFlow.Domain.Event;

namespace CreditFlow.Application.Proposal.Interface
{
    public interface IProposalAssessmentService
    {
        Task AssessProposalAsync(ClientRegisteredEvent clientEvent);
    }
}