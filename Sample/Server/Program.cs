using EasyNetQ;
using EfAudit;
using Microsoft.Extensions.Options;
using Server;
using Server.Controllers;
using Server.Handlers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogContext>((services, options) =>
{
    //    options.UseSqlite($"DataSource=catalog.db")
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddAuditInterceptor(services);
});

var rcs = builder.Configuration.GetConnectionString("rabbitMq");
var bus = RabbitHutch.CreateBus(rcs);
builder.Services.AddSingleton(bus);
builder.Services.
    AddEfAudit(
    builder.Configuration,
    AuditTrailController.AddRecords,
    RabbitMqEfAuditHandler.Handle
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//warm up - validate options
app.Services.GetRequiredService<IOptionsMonitor<AuditInterceptorOptions>>();

app.Lifetime.ApplicationStopping.Register(bus.Dispose);

using var scope = app.Services.CreateScope();
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
