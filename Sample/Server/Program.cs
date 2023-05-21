using EasyNetQ;
using EfAudit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Server;
using Server.Controllers;
using Server.Handlers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
var rcs = builder.Configuration.GetConnectionString("rabbitMq");
var bus = RabbitHutch.CreateBus(rcs);
services.AddSingleton(bus);


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Catalog API",
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

services.
    AddEfAudit(
    builder.Configuration,
    AuditTrailController.AddRecords,
    RabbitMqEfAuditHandler.Handle
);

services.AddDbContext<CatalogContext>((services, options) =>
{
    //    options.UseSqlite($"DataSource=catalog.db")
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddAuditInterceptor(services);
});

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
