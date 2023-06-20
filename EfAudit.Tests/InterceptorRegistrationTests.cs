using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfAudit.Tests
{
    public class InterceptorRegistrationTests
    {
        [Fact]
        public void EfInterceptorIsRegistered()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddEfAudit(builder.Configuration);

            builder.Services.AddDbContext<CatalogContext>((serviceProvider, options) =>
            {
                options.UseSqlite($"DataSource=interceptor-registration-tests.db")
                .AddEfAuditInterceptor(serviceProvider);
            });

            var app = builder.Build();

            var options = app.Services.GetRequiredService<DbContextOptions>();
            var ext = options.GetExtension<CoreOptionsExtension>();
            var interceptors = ext.Interceptors;
            interceptors.ShouldContain(x => x.GetType() == typeof(AuditSaveChangesInterceptor));
        }
    }

}
