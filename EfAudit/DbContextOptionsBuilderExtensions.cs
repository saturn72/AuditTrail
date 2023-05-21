
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddEfAuditInterceptor(
            this DbContextOptionsBuilder builder,
            IServiceProvider services)
        {
            builder.AddInterceptors(services.GetRequiredService<AuditSaveChangesInterceptor>());

            return builder;
        }
    }
}
