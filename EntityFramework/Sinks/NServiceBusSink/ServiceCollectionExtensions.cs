
using NServiceBusSink;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBusEfAudit(this IServiceCollection services)
        {
            services.AddEfAudit(EventSink.Handle);
            return services;
        }
    }
}
