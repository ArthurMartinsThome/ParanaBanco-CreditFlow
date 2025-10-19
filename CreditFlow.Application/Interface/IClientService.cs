using CreditFlow.Domain.Dto;

namespace CreditFlow.Application.Interface
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto> GetClientByIdAsync(Guid id);
        Task<ClientDto> RegisterClientAsync(ClientDto clientData);
        Task UpdateClientStatusAsync(ClientDto clientData);
    }
}