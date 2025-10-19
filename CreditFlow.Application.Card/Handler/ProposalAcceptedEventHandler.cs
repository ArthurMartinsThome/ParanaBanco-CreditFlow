using CreditFlow.Application.Card.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;

namespace CreditFlow.Application.Card.EventHandler
{
    public class ProposalAcceptedEventHandler :
        IEventHandler<ProposalApprovedEvent>,
        MediatR.INotificationHandler<ProposalApprovedEvent>
    {
        private readonly ICardIssuanceService _cardIssuanceService;

        public ProposalAcceptedEventHandler(ICardIssuanceService cardIssuanceService)
        {
            _cardIssuanceService = cardIssuanceService;
        }

        public Task Handle(ProposalApprovedEvent notification, CancellationToken cancellationToken)
        {
            return _cardIssuanceService.ProcessIssuanceAsync(notification);
        }
    }
}