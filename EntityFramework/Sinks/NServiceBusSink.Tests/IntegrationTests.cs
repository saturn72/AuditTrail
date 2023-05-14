using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using NServiceBusSink.Tests.Domain;
using NServiceBus;
using Moq;
using EfAudit;
using NServiceBus.Testing;
using Shouldly;

namespace NServiceBusSink.Tests
{
    public class IntegrationTests
    {
        private const string OutputDir = "./output/";
        private readonly TestableMessageSession _msgSession;
        private IServiceProvider _services;

        public IntegrationTests()
        {
            if (Directory.Exists(OutputDir))
                Directory.Delete(OutputDir, true);

            Directory.CreateDirectory(OutputDir);

            var builder = WebApplication.CreateBuilder();

            _msgSession = new TestableMessageSession();

            builder.Services.AddTransient<IMessageSession>(_ => _msgSession);
            builder.Services.AddNServiceBusEfAudit();

            builder.Services.AddDbContext<CatalogContext>((services, options) =>
            {
                options.UseSqlite("DataSource=catalog.db")
                .AddAuditInterceptor(services, options => { });
            });

            var app = builder.Build();

            using (var ctx = app.Services.GetRequiredService<CatalogContext>())

            {
                ctx.Database.EnsureDeletedAsync().Wait();
                ctx.Database.EnsureCreatedAsync().Wait();
            }
            _services = app.Services;
        }

        [Fact]
        public async Task EventSink_CRUD()
        {
            //create
            var created = new Product
            {
                Name = "p-1",
                Price = 1.11M,
                Categories = new List<Category>
                {
                    new Category
                    {
                        Name = "cat-1",
                    },
                    new Category
                    {
                        Name = "cat-2",
                    }
                },
            };

            using (var createScope = _services.CreateScope())
            {
                var ctx = createScope.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddAsync(created);
                await ctx.SaveChangesAsync();
            }
            _msgSession.PublishedMessages.Count().ShouldBe(1);
            var ar = _msgSession.PublishedMessages.First().ShouldBeOfType<AuditRecord>();
            ar.Exception.ShouldBeNull();
            ar.ShouldBeNull();

            //update
            using (var updateScope = _services.CreateScope())
            {
                var uCtx = updateScope.ServiceProvider.GetRequiredService<CatalogContext>();
                var updated = uCtx.Find<Product>(created.Id);
                updated.Name = "new name!!!";
                updated.Categories.Remove(updated.Categories.First());

                uCtx.Update(updated);
                await uCtx.SaveChangesAsync();
            }

            await Task.Delay(1000);
            //deleted
            using (var deleteScope = _services.CreateScope())
            {
                var dCtx = deleteScope.ServiceProvider.GetRequiredService<CatalogContext>();
                var r = dCtx.Find<Product>(created.Id);
                dCtx.Remove(r);
                await dCtx.SaveChangesAsync();
            }
            await Task.Delay(1000);
        }
    }
}