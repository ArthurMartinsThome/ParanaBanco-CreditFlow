using CreditFlow.Application.Interface;
using CreditFlow.Domain.Dto;
using CreditFlow.Domain.Request;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientService.GetAllClientsAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientById([FromRoute] Guid id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
                return NotFound(new { message = $"Client with ID {id} not found." });

            return Ok(client);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterClient([FromBody] RegisterClientRequest request)
        {
            var clientDto = new ClientDto
            {
                Name = request.Name,
                Document = request.Document,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                MonthlyIncome = request.MonthlyIncome,
                Id = null,
                Status = null,
                CreatedAt = null,
                UpdatedAt = null
            };

            try
            {
                var newClient = await _clientService.RegisterClientAsync(clientDto);

                return Accepted(new
                {
                    clientId = newClient.Id,
                    status = newClient.Status,
                    message = "Client registration accepted. Credit flow initiated asynchronously."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
            }
        }
    }
}