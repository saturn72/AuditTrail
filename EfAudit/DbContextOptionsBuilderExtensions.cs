
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddAuditInterceptor(
            this DbContextOptionsBuilder builder,
            IServiceProvider services)
        {
            builder.AddInterceptors(services.GetRequiredService<AuditSaveChangesInterceptor>());

            return builder;
        }
    }
}
