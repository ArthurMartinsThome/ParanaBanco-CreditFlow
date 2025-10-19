using CreditFlow.Application.Proposal.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using MediatR;

namespace CreditFlow.Application.Proposal.EventHandler
{
    public class ClientRegisteredEventHandler :
        IEventHandler<ClientRegisteredEvent>,
        INotificationHandler<ClientRegisteredEvent>
    {
        private readonly IProposalAssessmentService _assessmentService;

        public ClientRegisteredEventHandler(IProposalAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        public async Task Handle(ClientRegisteredEvent @event, CancellationToken cancellationToken)
        {
            await _assessmentService.AssessProposalAsync(@event);
        }
    }
}