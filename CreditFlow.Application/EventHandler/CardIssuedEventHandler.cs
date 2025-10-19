using CreditFlow.Application.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using MediatR;

namespace CreditFlow.Application.Client.EventHandler
{
    public class CardIssuedEventHandler :
        IEventHandler<CardIssuedEvent>,
        INotificationHandler<CardIssuedEvent>
    {
        private readonly IClientService _clientService;

        public CardIssuedEventHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task Handle(CardIssuedEvent notification, CancellationToken cancellationToken)
        {
            var newStatus = Domain.Enum.ECreditProposalStatus.Aprovado;

            try
            {
                await _clientService.UpdateClientStatusAsync(
                    new Domain.Dto.ClientDto
                    {
                        Id = notification.ClientId.GetValueOrDefault(),
                        CardNumber = notification.PartialCardNumber,
                        Status = newStatus,
                        Limit = notification.ApprovedLimit
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HANDLER FATAL] Erro ao processar CardIssuedEvent: {ex.Message}");
                throw;
            }
        }
    }
}