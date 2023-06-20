
using EfAudit.Extensions;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddEfAuditInterceptor(
            this DbContextOptionsBuilder builder,
            IServiceProvider services)
        {
            if (!DependencyManager.WasInitialized)
                DependencyManager.Init(services);

            using var scope = services.CreateScope();
            builder.AddInterceptors(scope.ServiceProvider.GetRequiredService<AuditSaveChangesInterceptor>());

            return builder;
        }
    }
}
