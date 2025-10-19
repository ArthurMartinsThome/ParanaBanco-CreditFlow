using CreditFlow.Application.Card.EventHandler;
using CreditFlow.Application.Card.Interface;
using CreditFlow.Application.Card.Service;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using CreditFlow.Integration.RabbitMq.Service;
using MediatR;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ProposalAcceptedEventHandler).Assembly)
        );

        services.AddSingleton<IRabbitMqClient>(provider =>
        {
            const string serviceName = "CreditFlow.MS.Card";

            var mediator = provider.GetRequiredService<IMediator>();
            var serviceProvider = provider.GetRequiredService<IServiceProvider>();

            return new RabbitMqClient(mediator, serviceProvider, serviceName);
        });

        services.AddScoped<ICardIssuanceService, CardIssuanceService>();

        services.AddHostedService<CreditFlow.MS.Card.Worker>();
    })
    .Build();

using (var scope = builder.Services.CreateScope())
{
    var rabbitMqClient = scope.ServiceProvider.GetRequiredService<IRabbitMqClient>();
    rabbitMqClient.Subscribe<ProposalApprovedEvent, ProposalAcceptedEventHandler>();
    rabbitMqClient.StartConsumer();
}

await builder.RunAsync();