using Microsoft.OpenApi.Models;
using Server;
using Server.Controllers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
builder.Services.AddEfAudit(AuditTrailController.AddRecords);
builder.Services.AddDbContext<CatalogContext>((services, options) =>
{
    options.UseSqlite($"DataSource=catalog.db")
    .AddAuditInterceptor(services, options => { });
});

var app = builder.Build();

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
