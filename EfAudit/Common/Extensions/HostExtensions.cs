using EfAudit.Extensions;

namespace Microsoft.Extensions.Hosting
{

    public static class HostExtensions
    {
        public static void UseEfAuditToHost(this IHost host)
        {
            DependencyManager.Init(host.Services);
        }
    }
}
