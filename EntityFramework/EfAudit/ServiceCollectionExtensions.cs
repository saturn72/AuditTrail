using EfAudit.Extractors;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfAudit(
            this IServiceCollection services,
            IConfiguration configuration,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        => AddEfAudit(services, configuration, typeof(DefaultDataChangedExtractor), handlers);

        public static IServiceCollection AddEfAudit(
            this IServiceCollection services,
            IConfiguration configuration,
            Type dataChangedExtractorType,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {
            if (!typeof(IDataChangedExtractor).IsAssignableFrom(dataChangedExtractorType))
                throw new InvalidOperationException($"The provided type \'{dataChangedExtractorType.FullName}\' is not of type {nameof(IDataChangedExtractor)}");

            services.TryAddSingleton(typeof(IDataChangedExtractor), dataChangedExtractorType);

            if (typeof(DefaultDataChangedExtractor).IsAssignableFrom(dataChangedExtractorType))
                services.AddHttpContextAccessor();

            services.AddScoped<AuditSaveChangesInterceptor>();
            services.Configure<AuditInterceptorOptions>(options =>
            {
                var section = configuration.GetSection(AuditInterceptorOptions.Section);
                options.Source = section["source"];
            });

            services.PostConfigure<AuditInterceptorOptions>(options => AuditInterceptorOptionsValidator.Validate(options));

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
