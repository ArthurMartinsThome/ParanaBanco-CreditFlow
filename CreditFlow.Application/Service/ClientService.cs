using CreditFlow.Application.Interface;
using CreditFlow.Domain.Dto;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.Document.Interface;
using CreditFlow.Integration.RabbitMq.Interface;

namespace CreditFlow.Application.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientDataSource _clientDataSource;
        private readonly IRabbitMqClient _messagePublisher;

        public ClientService(IClientDataSource clientDataSource, IRabbitMqClient messagePublisher)
        {
            _clientDataSource = clientDataSource;
            _messagePublisher = messagePublisher;
        }

        public async Task<ClientDto> RegisterClientAsync(ClientDto clientData)
        {
            clientData.Status = Domain.Enum.ECreditProposalStatus.EmAvaliacao;

            var newClient = await _clientDataSource.AddClientAsync(clientData);

            _messagePublisher.Publish(new ClientRegisteredEvent(
                ClientId: newClient.Id,
                Name: newClient.Name,
                Email: newClient.Email,
                PhoneNumber: newClient.PhoneNumber,
                Document: newClient.Document,
                MonthlyIncome: newClient.MonthlyIncome,
                Status: clientData.Status,
                CreatedAt: newClient.CreatedAt,
                UpdatedAt: newClient.UpdatedAt
            ));

            return newClient;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            return await _clientDataSource.GetAllClientsAsync();
        }
        public async Task<ClientDto> GetClientByIdAsync(Guid id)
        {
            return await _clientDataSource.GetClientByIdAsync(id);
        }

        public Task UpdateClientStatusAsync(ClientDto clientData)
        {
            return _clientDataSource.UpdateClientAsync(clientData);
        }
    }
}