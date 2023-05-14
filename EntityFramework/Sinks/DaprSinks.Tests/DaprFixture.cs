using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Dapr;
using EfAudit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DaprSinks.Tests
{
    public class DaprFixture : IDisposable
    {
        private readonly Process _process;
        public readonly WebApplication App;

        public DaprFixture()
        {
            //_process = Process.Start("dapr run --app-id dapr-sink-tests --dapr-http-port 3500 --resources-path ./components");

            var builder = WebApplication.CreateBuilder();
            builder.Services.AddDaprSinks();

            builder.Services.AddEfAudit(
                DaprPubSubSink.Handle);

            builder.Services.AddDbContext<CatalogContext>((services, options) =>
            {
                options.UseSqlite("DataSource=catalog.db")
                .AddAuditInterceptor(services, options => { });
            });

            App = builder.Build();
            App.UseCloudEvents();
            App.MapSubscribeHandler();

            App.Urls.Add("http://localhost:5001");
            App.Urls.Add("https://localhost:50011");

            App.MapPost("audit-trail", [Topic("auditpubsub", "audittrail")] (AuditRecord record) =>
            {
                return Task.FromResult(new OkObjectResult(record));
            });


            _ = App.RunAsync();

            using (var ctx = App.Services.GetRequiredService<CatalogContext>())
            {
                ctx.Database.EnsureDeletedAsync().Wait();
                ctx.Database.EnsureCreatedAsync().Wait();
            }
        }

        public void Dispose()
        {
            App?.StopAsync().Wait();
            App?.DisposeAsync().AsTask().Wait();

            _process?.Kill();
            _process?.Dispose();
        }
    }
}