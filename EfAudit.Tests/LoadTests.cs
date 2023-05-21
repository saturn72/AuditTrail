using AuditTrail.Common;
using Microsoft.Extensions.DependencyInjection;
using NBomber.CSharp;
using Shouldly;

namespace EfAudit.Tests
{
    public class LoadTests : IntegrationTestBase
    {
        static int i;
        static object l = new();
        static Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] Handlers =
            new Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] {
                (sp_, _, _) =>
                {

                lock(l)
                {
                        Interlocked.Increment(ref i);
                }
                return Task.CompletedTask;
            }
            };

        public LoadTests() : base(Handlers)
        {
        }

        [Fact]
        public async Task Do()
        {
            int iteration = 0;
            for (; iteration < 10000; iteration++)
            {
                var p = new Product
                {
                    Name = "product_" + iteration,
                };

                using (var scope = Services.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<CatalogContext>();
                    await ctx.AddAsync(p);
                    await ctx.SaveChangesAsync();
                }
            }
            i.ShouldBe(iteration);
        }
        [Theory]
        [InlineData(10000)]
        public void CreateProduct(int total)
        {
            var scenario = Scenario.Create($"Create {total} products", async context =>
            {
                var p = new Product
                {
                    Name = "product_" + i,
                };
                context.Logger.Information("BEFORE creating product: " + p.Name);
                using (var scope = Services.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<CatalogContext>();
                    await ctx.AddAsync(p);
                    await ctx.SaveChangesAsync();
                }
                context.Logger.Information("AFTER creating product: " + p.Name);
                return Response.Ok();
            })
          .WithLoadSimulations(
              Simulation.Inject(rate: total,
                                interval: TimeSpan.FromMilliseconds(10),
                                during: TimeSpan.FromSeconds(300))
          );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            i.ShouldBe(total);
        }
    }
}
