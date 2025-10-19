using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CreditFlow.Integration.RabbitMq.Service
{
    public class RabbitMqClient : IRabbitMqClient, IDisposable
    {
        private const string ExchangeName = "bank.events";
        private const string ExchangeType = "topic";

        private readonly IConnection _connection;
        private readonly IModel _publishChannel;

        private IModel _consumeChannel;

        private readonly IServiceProvider _serviceProvider;
        private readonly string _serviceName;
        private readonly string _queueName;

        private readonly Dictionary<string, Type> _eventTypeMap = new Dictionary<string, Type>();
        private readonly Dictionary<Type, Type> _handlerTypeMap = new Dictionary<Type, Type>();

        private EventingBasicConsumer _consumer;

        public RabbitMqClient(IMediator mediator, IServiceProvider serviceProvider, string serviceName)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            _queueName = $"{_serviceName}.queue";

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true
            };

            try
            {
                _connection = factory.CreateConnection();
                _publishChannel = _connection.CreateModel();
                _publishChannel.ExchangeDeclare(ExchangeName, ExchangeType, durable: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ ERROR] Falha ao conectar ou inicializar o RabbitMQ: {ex.Message}");
                throw new InvalidOperationException("Falha ao inicializar o RabbitMQ Client. Verifique a disponibilidade do broker.", ex);
            }
        }

        public void Publish<T>(T @event) where T : BaseEvent
        {
            if (_publishChannel == null || !_connection.IsOpen)
                throw new InvalidOperationException("RabbitMQ connection or publish channel is not open.");

            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var routingKey = @event.GetType().FullName ?? @event.GetType().Name;

            var properties = _publishChannel.CreateBasicProperties();
            properties.Persistent = true;
            properties.CorrelationId = @event.CorrelationId.ToString();

            _publishChannel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );
        }

        public void Subscribe<T, TH>()
            where T : BaseEvent
            where TH : IEventHandler<T>
        {
            var eventType = typeof(T);
            var handlerType = typeof(TH);

            var routingKey = eventType.FullName ?? eventType.Name;

            if (!_eventTypeMap.ContainsKey(routingKey))
            {
                _eventTypeMap.Add(routingKey, eventType);
                _handlerTypeMap.Add(eventType, handlerType);
            }
        }

        public void StartConsumer()
        {
            if (!_eventTypeMap.Any())
            {
                Console.WriteLine("[RabbitMQ WARN] Consumidor iniciado, mas nenhum evento foi subscrito via Subscribe<T, TH>().");
                return;
            }

            InitializeConsumerChannel();

            foreach (var key in _eventTypeMap.Keys)
            {
                _consumeChannel.QueueBind(
                    queue: _queueName,
                    exchange: ExchangeName,
                    routingKey: key
                );
            }

            _consumer = new EventingBasicConsumer(_consumeChannel);
            _consumer.Received += async (model, ea) =>
            {
                await ProcessMessage(ea);
            };

            _consumeChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer
            );
        }

        private async Task ProcessMessage(BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = eventArgs.RoutingKey;
            var deliveryTag = eventArgs.DeliveryTag;

            try
            {
                if (!_eventTypeMap.TryGetValue(routingKey, out Type eventType))
                {
                    Console.WriteLine($"[RabbitMQ WARN] Tipo de evento não subscrito ou desconhecido: {routingKey}. ACK.");
                    _consumeChannel.BasicAck(deliveryTag, false);
                    return;
                }

                var @event = JsonSerializer.Deserialize(message, eventType) as BaseEvent;

                if (@event != null)
                    await HandleEvent(@event);

                _consumeChannel.BasicAck(deliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ FATAL] Erro ao processar mensagem do RabbitMQ ({routingKey}). Erro: {ex.Message}. Enviando NACK para DLQ.");
                _consumeChannel.BasicNack(deliveryTag, false, false);
            }
        }

        private async Task HandleEvent(BaseEvent @event)
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(@event);
        }

        private void InitializeConsumerChannel()
        {
            if (_connection == null || !_connection.IsOpen)
                throw new InvalidOperationException("RabbitMQ connection is not open.");

            _consumeChannel = _connection.CreateModel();
            _consumeChannel.BasicQos(0, 1, false);

            var dlxName = $"{_serviceName}.DLX";
            var dlqName = $"{_queueName}.dlq";

            _consumeChannel.ExchangeDeclare(dlxName, "fanout", durable: true);
            _consumeChannel.QueueDeclare(dlqName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _consumeChannel.QueueBind(
                queue: dlqName,
                exchange: dlxName,
                routingKey: ""
            );

            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", dlxName },
                { "x-dead-letter-routing-key", "dead-message" }
            };

            _consumeChannel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
            );
        }

        public void Dispose()
        {
            _consumeChannel?.Close();
            _publishChannel?.Close();
            _connection?.Close();
            _consumeChannel?.Dispose();
            _publishChannel?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}