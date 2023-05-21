using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using AuditTrail.Common;

namespace EfAudit.Tests
{
    public abstract class IntegrationTestBase
    {
        protected const string OutputDir = "./output";

        protected readonly IServiceProvider Services;
        protected IntegrationTestBase(Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {
            if (Directory.Exists(OutputDir))
                Directory.Delete(OutputDir, true);

            Directory.CreateDirectory(OutputDir);

            var builder = WebApplication.CreateBuilder();

            builder.Services.AddEfAudit(builder.Configuration, handlers);

            builder.Services.AddDbContext<CatalogContext>((services, options) =>
            {
                options.UseSqlite($"DataSource=catalog.db")
                .AddEfAuditInterceptor(services);
            });

            var app = builder.Build();

            using (var ctx = app.Services.GetRequiredService<CatalogContext>())
            {
                ctx.Database.EnsureDeletedAsync().Wait();
                ctx.Database.EnsureCreatedAsync().Wait();
            }
            Services = app.Services;
        }

        protected static async Task EfAuditJsonHandler(
            IServiceProvider services,
            AuditRecord record,
            CancellationToken cancellationToken)
        {
            var l = new List<object>();
            foreach (var e in record.Entities)
            {
                var diff = e.ModifiedProperties.Select(x => new
                {
                    attributeName = x.Name,
                    currentValue = x.CurrentValue,
                    previousValue = x.OriginalValue,
                }).ToList();

                var o = new
                {
                    action = e.State,
                    type = e.TypeName,
                    value = e.Value,
                    diff
                };
                l.Add(o);
            }
            if (!l.Any())
                return;

            //to prevent multiple files with same name
            var filename = Path.Combine(OutputDir, record.Uuid + ".json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var r = new
            {
                version = "v1",
                transaction = new
                {
                    success = record.Success,
                    id = record.Uuid,
                    startedOnUtc = record.StartedOnUtc,
                    endedOnUtc = record.EndedOnUtc,
                },
                error = record.Exception?.InnerException?.Message,
                trail = l
            };

            var jsonString = JsonSerializer.Serialize(r, options);
            await File.WriteAllTextAsync(filename, jsonString, cancellationToken);
        }
    }
}