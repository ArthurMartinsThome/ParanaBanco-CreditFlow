using CreditFlow.Domain.Converter;
using CreditFlow.Domain.Dto;
using CreditFlow.Integration.Document.Interface;
using CreditFlow.Integration.Document.Model;
using System.Text.Json;

namespace CreditFlow.Integration.Document.DataSource
{
    public class ClientDataSource : IClientDataSource
    {
        private readonly string _filePath;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ClientDataSource(string clientDataPath)
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, clientDataPath);

            var directory = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        private ClientDto DtoMapper(Model.Client obj)
        {
            return new ClientDto
            {
                Id = obj.id,
                Name = obj.name,
                Email = obj.email,
                PhoneNumber = obj.phone_number,
                Document = obj.document,
                Status = obj.status,
                FailureReason = obj.failure_reason,
                CardNumber = obj.card_number,
                MonthlyIncome = obj.monthly_income,
                Limit = obj.limit,
                CreatedAt = obj.created_at,
                UpdatedAt = obj.updated_at
            };
        }

        private Model.Client EntityMapper(ClientDto obj)
        {
            return new Model.Client
            {
                id = obj.Id,
                name = obj.Name,
                email = obj.Email,
                phone_number = obj.PhoneNumber,
                document = obj.Document,
                status = obj.Status,
                failure_reason = obj.FailureReason,
                card_number = obj.CardNumber,
                monthly_income = obj.MonthlyIncome,
                limit = obj.Limit,
                created_at = obj.CreatedAt,
                updated_at = obj.UpdatedAt
            };
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var clients = JsonSerializer.Deserialize<IEnumerable<Client>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return clients.Select(DtoMapper).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"[ClientDataSource] Erro ao buscar clientes: {ex.Message}", ex);
            }
        }

        public async Task<ClientDto> GetClientByIdAsync(Guid id)
        {
            try
            {
                var clients = await GetAllClientsAsync();
                return clients.FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"[ClientDataSource] Erro ao buscar cliente pelo ID-{id}: {ex.Message}", ex);
            }
        }

        public async Task<ClientDto> AddClientAsync(ClientDto client)
        {
            await _semaphore.WaitAsync();
            try
            {
                var clients = await GetAllClientsAsync();

                client.Id = Guid.NewGuid();
                client.CreatedAt = DateTime.UtcNow;

                var clientList = clients.ToList();
                clientList.Add(client);

                var json = JsonSerializer.Serialize(clientList.Select(EntityMapper), new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(_filePath, json);

                return client;
            }
            catch (Exception ex)
            {
                throw new Exception($"[ClientDataSource] Erro ao adicionar cliente-{client.Name}: {ex.Message}", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<ClientDto> UpdateClientAsync(ClientDto client)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (client == null || client.Id == null)
                    throw new InvalidOperationException($"[ClientDataSource] Dados do cliente estão incompletos.");
                
                var clients = await GetAllClientsAsync();
                var clientList = clients.ToList();
                var existingClient = clientList.FirstOrDefault(c => c.Id == client.Id);

                if (existingClient == null)
                    throw new InvalidOperationException($"[ClientDataSource] Cliente com ID {client.Id} não encontrado na base.");

                if (client.Name != null)
                    existingClient.Name = client.Name;
                if (client.Email != null)
                    existingClient.Email = client.Email;
                if (client.PhoneNumber != null)
                    existingClient.PhoneNumber = client.PhoneNumber;
                if (client.Document != null)
                    existingClient.Document = client.Document;
                if (client.MonthlyIncome != null)
                    existingClient.MonthlyIncome = client.MonthlyIncome;
                if (client.Status != null)
                    existingClient.Status = client.Status;
                if (client.FailureReason != null)
                    existingClient.FailureReason = client.FailureReason;
                if (client.CardNumber != null)
                    existingClient.CardNumber = client.CardNumber;
                if (client.Limit != null)
                    existingClient.Limit = client.Limit;

                existingClient.UpdatedAt = DateTime.UtcNow;

                var json = JsonSerializer.Serialize(clients.Select(EntityMapper), new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new DecimalTwoPlacesConverter() }
                });

                await File.WriteAllTextAsync(_filePath, json);

                return client;
            }
            catch (Exception ex)
            {
                if(client == null)
                    throw new Exception($"Erro ao atualizar cliente, client is null: {ex.Message}", ex);

                if(client.Id == null)
                    throw new Exception($"Erro ao atualizar cliente, client.Id is null: {ex.Message}", ex);

                throw new Exception($"Erro ao atualizar cliente-{client.Id}: {ex.Message}", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}