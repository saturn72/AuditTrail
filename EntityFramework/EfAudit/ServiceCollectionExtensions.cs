

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfAudit(this IServiceCollection services,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {

            services.TryAddSingleton<DefaultEventBus>();
            services.TryAddSingleton<IEventBus>(sp =>
            {
                var deb = sp.GetRequiredService<DefaultEventBus>();
                foreach (var h in handlers)
                {
                    deb.Subscribe(h);
                }
                return deb;
            });
            return services;
        }
    }
}
