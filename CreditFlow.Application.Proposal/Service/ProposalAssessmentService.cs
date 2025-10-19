using CreditFlow.Application.Proposal.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;

namespace CreditFlow.Application.Proposal.Service
{
    public class ProposalAssessmentService : IProposalAssessmentService
    {
        private readonly IRabbitMqClient _rabbitMqClient;

        public ProposalAssessmentService(IRabbitMqClient rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient;
        }

        public async Task AssessProposalAsync(ClientRegisteredEvent clientEvent)
        {
            if (clientEvent.MonthlyIncome > 2000.00m)
            {
                decimal proposedLimit = clientEvent.MonthlyIncome.Value * 0.30m;

                var acceptedEvent = new ProposalApprovedEvent(
                    clientEvent.ClientId,
                    proposedLimit
                );

                _rabbitMqClient.Publish(acceptedEvent);
            }
            else
            {
                var rejectedEvent = new ProposalRejectedEvent(
                    clientEvent.ClientId,
                    "Renda insuficiente para aprovacao minima (abaixo de R$ 2.000,00)."
                );

                _rabbitMqClient.Publish(rejectedEvent);
            }
        }
    }
}