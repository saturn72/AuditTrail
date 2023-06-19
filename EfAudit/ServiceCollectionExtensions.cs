using EfAudit.Common.Extractors;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEfAudit(
            this IServiceCollection services,
            IConfiguration configuration,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        => AddEfAudit(services, configuration, typeof(DefaultAuditRecordToAuditMessageMapper), handlers);

        public static IServiceCollection AddEfAudit(
            this IServiceCollection services,
            IConfiguration configuration,
            Type mapperType,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {
            if (!typeof(IAuditRecordToAuditMessageMapper).IsAssignableFrom(mapperType))
                throw new InvalidOperationException($"The provided type \'{mapperType.FullName}\' is not of type {nameof(IAuditRecordToAuditMessageMapper)}");

            services.TryAddSingleton(typeof(IAuditRecordToAuditMessageMapper), mapperType);

            if (typeof(DefaultAuditRecordToAuditMessageMapper).IsAssignableFrom(mapperType))
                services.AddHttpContextAccessor();

            services.AddScoped<AuditSaveChangesInterceptor>();
            services.Configure<AuditInterceptorOptions>(options =>
            {
                var section = configuration.GetSection(AuditInterceptorOptions.Section);

                var tmpSource = section["source"];
                options.Source = tmpSource.HasValue() ? tmpSource : Assembly.GetEntryAssembly()?.GetName().Name;
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
