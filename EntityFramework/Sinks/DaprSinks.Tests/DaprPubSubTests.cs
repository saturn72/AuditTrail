using Server;
using Server.Domain;

namespace DaprSinks.Tests
{
    [Collection(DaprCollection.Name)]
    public class DaprPubSubTests
    {
        private readonly DaprFixture _fixture;

        public DaprPubSubTests(DaprFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public async Task SendsAuditRecordToPubSub()
        {
            //craete categories
            var cat1 = new Category { Name = "cat-1", };
            var cat2 = new Category { Name = "cat-2", };

            using (var sc = _fixture.App.Services.CreateScope())
            {
                var ctx = sc.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddRangeAsync(new List<Category> { cat1, cat2 });
                await ctx.SaveChangesAsync();
            }

            await Task.Delay(10000);
        }
    }
}