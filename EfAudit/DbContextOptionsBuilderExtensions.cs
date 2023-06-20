
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

            builder.AddInterceptors(services.GetRequiredService<AuditSaveChangesInterceptor>());

            return builder;
        }
    }
}
