using CreditFlow.Application.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using MediatR;

namespace CreditFlow.Application.Client.EventHandler
{
    public class CardIssuanceFailedEventHandler :
        IEventHandler<CardIssuanceFailedEvent>,
        INotificationHandler<CardIssuanceFailedEvent>
    {
        private readonly IClientService _clientService;

        public CardIssuanceFailedEventHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task Handle(CardIssuanceFailedEvent notification, CancellationToken cancellationToken)
        {
            var newStatus = Domain.Enum.ECreditProposalStatus.FalhaEmissao;

            try
            {
                await _clientService.UpdateClientStatusAsync(
                    new Domain.Dto.ClientDto
                    {
                        Id = notification.ClientId.GetValueOrDefault(),
                        FailureReason = notification.FailureReason,
                        Status = newStatus
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HANDLER FATAL] Erro ao processar CardIssuanceFailedEvent: {ex.Message}");
                throw;
            }
        }
    }
}