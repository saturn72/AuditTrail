
using EfAudit;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddAuditInterceptor(
            this DbContextOptionsBuilder builder,
            IServiceProvider services,
            Action<AuditInterceptorOptions> configure)
        {
            var options = new AuditInterceptorOptions();
            configure(options);


            var interceptor = new AuditSaveChangesInterceptor(services, options);
            builder.AddInterceptors(interceptor);

            return builder;
        }
    }
}
