using CreditFlow.Application.Client.EventHandler;
using CreditFlow.Application.Interface;
using CreditFlow.Application.Service;
using CreditFlow.Domain.Event;
using CreditFlow.Integration.Document.DataSource;
using CreditFlow.Integration.Document.Interface;
using CreditFlow.Integration.RabbitMq.Interface;
using CreditFlow.Integration.RabbitMq.Service;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

var clientDataPath = builder.Configuration.GetValue<string>("PersistenceSettings:ClientDataPath")
    ?? throw new InvalidOperationException("PersistenceSettings:ClientDataPath não configurado.");
builder.Services.AddSingleton<IClientDataSource>(provider => new ClientDataSource(clientDataPath));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ProposalRejectedEventHandler).Assembly)
);

builder.Services.AddSingleton<IRabbitMqClient>(provider =>
{
    const string serviceName = "CreditFlow.MS.Client";
    var mediator = provider.GetRequiredService<IMediator>();
    return new RabbitMqClient(mediator, provider, serviceName);
});

builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        var rabbitClient = app.Services.GetRequiredService<IRabbitMqClient>();

        rabbitClient.Subscribe<ProposalRejectedEvent, ProposalRejectedEventHandler>();
        rabbitClient.Subscribe<CardIssuedEvent, CardIssuedEventHandler>();
        rabbitClient.Subscribe<CardIssuanceFailedEvent, CardIssuanceFailedEventHandler>();

        rabbitClient.StartConsumer();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[RABBITMQ FATAL] Falha ao iniciar o consumidor: {ex.Message}");
    }
});

app.Run();