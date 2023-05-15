using AuditTrail.Common;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EfAudit.Tests
{
    public class FunctionalTests : IntegrationTestBase
    {
        protected List<AuditRecord> Records => new();

        //static new Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers = new[] { EfAuditJsonHandler };
        public FunctionalTests() : base(null) { }
        protected Task EfAuditInMemoryHandler(
          IServiceProvider services,
          AuditRecord record,
          CancellationToken cancellationToken)
        {
            Records.Add(record);
            return Task.CompletedTask;
        }
        [Fact]
        public async Task CreateProductWithMultipleCategoriesAndRemoveCategories()
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

            using (var createScope = Services.CreateScope())
            {
                var ctx = createScope.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddAsync(created);
                await ctx.SaveChangesAsync();
            }
            Records.Count().ShouldBe(1);
            Records.Last().Entities.Count.ShouldBe(5);
            await Task.Delay(1000);

            var cat1 = created.Categories.First();
            //update
            using (var updateScope = Services.CreateScope())
            {
                var uCtx = updateScope.ServiceProvider.GetRequiredService<CatalogContext>();
                var updated = uCtx.Find<Product>(created.Id);
                updated.Categories = new List<Category> { cat1 };
                uCtx.Update(updated);
                await uCtx.SaveChangesAsync();
            }
            await Task.Delay(1000);

            Records.Count().ShouldBe(2);
            Records.Last().Entities.Count.ShouldBe(2);
            await Task.Delay(3000);
        }

        [Fact]
        public async Task CreateMultiple()
        {
            //craete categories
            var cat1 = new Category { Name = "cat-1", };
            var cat2 = new Category { Name = "cat-2", };

            using (var sc = Services.CreateScope())
            {
                var ctx = sc.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddRangeAsync(new List<Category> { cat1, cat2 });
                await ctx.SaveChangesAsync();
            }
            Records.Count().ShouldBe(1);
            Records.Last().Entities.Count.ShouldBe(2);
            //create

            using (var createScope = Services.CreateScope())
            {
                var ctx = createScope.ServiceProvider.GetRequiredService<CatalogContext>();
                var tc1 = ctx.Find<Category>(cat1.Id);
                var tc2 = ctx.Find<Category>(cat2.Id);

                var t = new Product
                {
                    Name = "p-1",
                    Price = 1.11M,
                    Categories = new List<Category>(new[] { tc1, tc2 }),
                };

                await ctx.AddRangeAsync(t);
                await ctx.SaveChangesAsync();

                Records.Last().Entities.Count.ShouldBe(2);

                var created = new List<Product>{
                new Product
                {
                    Name = "p-1",
                    Price = 1.11M,
                    Categories =  new List<Category>( new[] { tc1, tc2 } ),
                },
                new Product
                {
                    Name = "p-2",
                    Price = 2.22M,
                    Categories = new List<Category> (new[] { tc1 })
                    },
                };

                await ctx.AddRangeAsync(created);
                await ctx.SaveChangesAsync();
            }
            Records.Count().ShouldBe(2);
            Records.Last().Entities.Count.ShouldBe(2);
        }

        [Fact]
        public async Task CreateProductAndUpdateProductsCategoryName()
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

            using (var createScope = Services.CreateScope())
            {
                var ctx = createScope.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddAsync(created);
                await ctx.SaveChangesAsync();
            }
            await Task.Delay(1000);


#warning add assertion here
            //update
            using (var updateScope = Services.CreateScope())
            {
                created.Categories.First().Name = "new category name";
                var uCtx = updateScope.ServiceProvider.GetRequiredService<CatalogContext>();

                uCtx.Update(created);
                await uCtx.SaveChangesAsync();
            }

            await Task.Delay(1000);


        }
        [Fact]
        public async Task CreateUpdateDeleteAsync()
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

            using (var createScope = Services.CreateScope())
            {
                var ctx = createScope.ServiceProvider.GetRequiredService<CatalogContext>();
                await ctx.AddAsync(created);
                await ctx.SaveChangesAsync();
            }
            await Task.Delay(1000);
            //update
            using (var updateScope = Services.CreateScope())
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
            using (var deleteScope = Services.CreateScope())
            {
                var dCtx = deleteScope.ServiceProvider.GetRequiredService<CatalogContext>();
                var r = dCtx.Find<Product>(created.Id);
                dCtx.Remove(r);
                await dCtx.SaveChangesAsync();
            }
            await Task.Delay(1000);

            Records.Count().ShouldBe(3);
            Records.ElementAt(0).Entities.Count.ShouldBe(2);
            Records.ElementAt(1).Entities.Count.ShouldBe(1);
            Records.ElementAt(2).Entities.Count.ShouldBe(1);
        }
    }
}