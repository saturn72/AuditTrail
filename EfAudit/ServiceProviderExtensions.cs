using Microsoft.EntityFrameworkCore.Infrastructure;

namespace System
{
    public static class ServiceProviderExtensions
    {
        public static void ValidateEfAudit(this IServiceProvider services)
        {
            using var scope = services.CreateAsyncScope();
            var options = scope.ServiceProvider.GetRequiredService<DbContextOptions>();
            var ext = options.GetExtension<CoreOptionsExtension>();
            var interceptors = ext.Interceptors;
            if (!interceptors.Any(x => x.GetType() == typeof(AuditSaveChangesInterceptor)))
                throw new InvalidOperationException("EfAudit was not registered properly. see: https://github.com/saturn72/EfAudit for more details");
        }
    }
}
