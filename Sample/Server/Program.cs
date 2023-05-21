using EasyNetQ;
using EfAudit;
using Microsoft.Extensions.Options;
using Server;
using Server.Controllers;
using Server.Handlers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//add efAudit services.
// this is an example for using multiple audit handlers
builder.Services.AddEfAudit(
    builder.Configuration,
    AuditTrailController.AddRecords, //first audit handler
    RabbitMqEfAuditHandler.Handle // second audit handler
);

//register EF DbContext
builder.Services.AddDbContext<CatalogContext>((services, options) =>
{
    //    options.UseSqlite($"DataSource=catalog.db")
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        //add efAuditInterceptor
        .AddAuditInterceptor(services);
});

//configure easynetq (rabbitmq client)
var rcs = builder.Configuration.GetConnectionString("rabbitMq");
var bus = RabbitHutch.CreateBus(rcs);
builder.Services.AddSingleton(bus);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//warm up - validate options(optional)
app.Services.GetRequiredService<IOptionsMonitor<AuditInterceptorOptions>>();

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

app.MapControllers();

app.Run();
