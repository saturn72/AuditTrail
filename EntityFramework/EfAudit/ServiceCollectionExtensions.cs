

using EfAudit.Extractors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfAudit(this IServiceCollection services,
    params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        => AddEfAudit(services, typeof(DefaultDataChangedExtractor), handlers);

        public static IServiceCollection AddEfAudit(this IServiceCollection services,
            Type dataChangedExtractorType,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {

            if (!typeof(IDataChangedExtractor).IsAssignableFrom(dataChangedExtractorType))
                throw new InvalidOperationException($"The provided type \'{dataChangedExtractorType.FullName}\' is not of type {nameof(IDataChangedExtractor)}");

            services.TryAddSingleton(typeof(IDataChangedExtractor), dataChangedExtractorType);
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
