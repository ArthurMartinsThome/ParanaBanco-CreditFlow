using CreditFlow.Application.Card.Interface;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using Microsoft.Extensions.Logging;

namespace CreditFlow.Application.Card.Service
{
    public class CardIssuanceService : ICardIssuanceService
    {
        private readonly IRabbitMqClient _rabbitMqClient;
        private readonly ILogger<CardIssuanceService> _logger;
        private readonly Random _random = new Random();

        public CardIssuanceService(IRabbitMqClient rabbitMqClient, ILogger<CardIssuanceService> logger)
        {
            _rabbitMqClient = rabbitMqClient;
            _logger = logger;
        }

        public async Task ProcessIssuanceAsync(ProposalApprovedEvent acceptedEvent)
        {
            // Simula uma falha externa
            if (_random.Next(1, 101) <= 20)
            {
                var failureReason = "Falha no sistema de terceiros na hora da emissao.";

                _rabbitMqClient.Publish(new CardIssuanceFailedEvent(
                    acceptedEvent.ClientId,
                    failureReason
                ));
                return;
            }

            var partialCardNumber = $"**** **** **** {_random.Next(1000, 9999)}";

            _rabbitMqClient.Publish(new CardIssuedEvent(
                acceptedEvent.ClientId,
                partialCardNumber,
                "APROVADO",
                acceptedEvent.ApprovedLimit
            ));
        }
    }
}