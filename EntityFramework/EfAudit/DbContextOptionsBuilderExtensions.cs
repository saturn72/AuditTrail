
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddAuditInterceptor(
            this DbContextOptionsBuilder builder,
            IServiceProvider services)
        {
            var interceptor = services.GetRequiredService<AuditSaveChangesInterceptor>();
            builder.AddInterceptors(interceptor);

            return builder;
        }
    }
}
