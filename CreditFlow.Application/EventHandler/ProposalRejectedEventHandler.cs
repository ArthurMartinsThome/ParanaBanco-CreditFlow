using CreditFlow.Application.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using MediatR;

namespace CreditFlow.Application.Client.EventHandler
{
    public class ProposalRejectedEventHandler :
        IEventHandler<ProposalRejectedEvent>,
        INotificationHandler<ProposalRejectedEvent>
    {
        private readonly IClientService _clientService;

        public ProposalRejectedEventHandler(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task Handle(ProposalRejectedEvent notification, CancellationToken cancellationToken)
        {
            var newStatus = Domain.Enum.ECreditProposalStatus.Reprovado;

            try
            {
                await _clientService.UpdateClientStatusAsync(
                    new Domain.Dto.ClientDto
                    {
                        Id = notification.ClientId.GetValueOrDefault(),
                        FailureReason = notification.Reason,
                        Status = newStatus
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HANDLER FATAL] Erro ao processar ProposalRejectedEvent: {ex.Message}");
                throw;
            }
        }
    }
}