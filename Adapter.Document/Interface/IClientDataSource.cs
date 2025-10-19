using CreditFlow.Domain.Dto;

namespace CreditFlow.Integration.Document.Interface
{
    public interface IClientDataSource
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(Guid id);
        Task<ClientDto> AddClientAsync(ClientDto client);
        Task<ClientDto> UpdateClientAsync(ClientDto client);
    }
}