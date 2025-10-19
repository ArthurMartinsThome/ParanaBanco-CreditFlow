using CreditFlow.Application.Proposal.EventHandler;
using CreditFlow.Application.Proposal.Interface;
using CreditFlow.Application.Proposal.Service;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.RabbitMq.Interface;
using CreditFlow.Integration.RabbitMq.Service;
using MediatR;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ClientRegisteredEventHandler).Assembly)
        );

        services.AddSingleton<IRabbitMqClient>(provider =>
        {
            const string serviceName = "CreditFlow.MS.Proposal";

            var mediator = provider.GetRequiredService<IMediator>();
            var serviceProvider = provider.GetRequiredService<IServiceProvider>();

            return new RabbitMqClient(mediator, serviceProvider, serviceName);
        });

        services.AddScoped<IProposalAssessmentService, ProposalAssessmentService>();
        services.AddHostedService<CreditFlow.MS.Proposal.Worker>();
    })
    .Build();

using (var scope = builder.Services.CreateScope())
{
    var rabbitMqClient = scope.ServiceProvider.GetRequiredService<IRabbitMqClient>();
    rabbitMqClient.Subscribe<ClientRegisteredEvent, ClientRegisteredEventHandler>();

    rabbitMqClient.StartConsumer();
}

await builder.RunAsync();