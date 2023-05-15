using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("EfAudit");
    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
    transport.UseConventionalRoutingTopology(QueueType.Quorum);
    transport.ConnectionString(context.Configuration.GetConnectionString("rabbitMq"));

    endpointConfiguration.EnableInstallers();

    return endpointConfiguration;
});

// Add services to the container.
var app = builder.Build();
app.MapGet("/", () => AuditMessageHandler.OnGet());
app.Run();
