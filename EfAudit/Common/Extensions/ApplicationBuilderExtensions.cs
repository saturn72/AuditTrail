using EfAudit.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseEfAudit(this IApplicationBuilder builder)
        {
            DependencyManager.SubscribeAllEfAuditRecordHandlers(builder.ApplicationServices);
        }
    }
}
