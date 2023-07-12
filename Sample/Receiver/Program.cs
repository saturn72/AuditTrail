
using AuditTrail.Common;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);

var rcs = builder.Configuration.GetConnectionString("rabbitMq");
var bus = RabbitHutch.CreateBus(rcs);
builder.Services.AddSingleton(bus);
builder.Services.AddScoped<AuditMessageHandler>();
// Add services to the container.
var app = builder.Build();

bus.PubSub.Subscribe<IAuditMessageHandler.OutgoingMessage>("default", msg =>
{
    using var scope = app.Services.CreateScope();
    var ah = scope.ServiceProvider.GetRequiredService<AuditMessageHandler>();
    ah.Handle(msg);
});
app.Lifetime.ApplicationStopping.Register(bus.Dispose);

app.MapGet("/", () => AuditMessageHandler.OnGet());
app.Run();
