using AuditTrail.Common;
using EasyNetQ;
using EfAudit;
using Microsoft.Extensions.Options;
using Server;
using Server.Controllers;
using Server.Handlers;

var builder = WebApplication.CreateBuilder(args);

// options 1:
//add efAudit services.
// this is an example for using multiple audit handlers
builder.Services.AddEfAudit<RabbitMqEfAuditHandler>(builder.Configuration);

//register EF DbContext
builder.Services.AddDbContext<CatalogContext>((services, options) =>
{
    //    options.UseSqlite($"DataSource=catalog.db")
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        //add efAuditInterceptor
        .AddEfAuditInterceptor(services);
});


//options 2: use dependency registrar
builder.Services.AddTransient<IAuditRecordHandler, AuditTrailController>();

//options 2: you may not explicit define the type
builder.Services.AddTransient<YetAnotherAuditRecordHandler>();

//configure easynetq (rabbitmq client)
var rcs = builder.Configuration.GetConnectionString("rabbitMq");
var bus = RabbitHutch.CreateBus(rcs);
builder.Services.AddSingleton(bus);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// optional - validate EfAudit interceptor was added
app.Services.ValidateEfAudit();

//warm up - validate options(optional)
app.Services.GetRequiredService<IOptionsMonitor<EfAuditOptions>>();

app.Lifetime.ApplicationStopping.Register(bus.Dispose);

using var scope = app.Services.CreateScope();
//recreate the database
using (var ctx = scope.ServiceProvider.GetRequiredService<CatalogContext>())
{
    ctx.Database.EnsureDeletedAsync().Wait();
    ctx.Database.EnsureCreatedAsync().Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }