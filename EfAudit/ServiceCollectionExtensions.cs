using EfAudit.Common.Mappers;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds EfAudit using default services and handlers
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddEfAudit<TAuditMessageHandler>(
           this IServiceCollection services,
           IConfiguration configuration)
           where TAuditMessageHandler : class, IAuditMessageHandler
        {
            services.AddScoped<IAuditMessageHandler, TAuditMessageHandler>();
            services.TryAddScoped<IAuditMessageSubjectBuilder, HttpContextAuditMessageSubjectBuilder>();
            services.TryAddSingleton<IAuditRecordToAuditMessageMapper, DefaultAuditRecordToAuditMessageMapper>();
            services.AddHttpContextAccessor();

            return AddEfAuditCore(services, configuration, DefaultAuditMessageProcessor.Handle);
        }

        public static IServiceCollection AddEfAuditCore(
            this IServiceCollection services,
            IConfiguration configuration,
            params Func<IServiceProvider, AuditRecord, CancellationToken, Task>[] handlers)
        {
            services.AddScoped<AuditSaveChangesInterceptor>();
            services.Configure<EfAuditOptions>(options =>
            {
                var section = configuration.GetSection(EfAuditOptions.Section);

                var tmpSource = section["source"];
                options.Source = tmpSource.HasValue() ? tmpSource : Assembly.GetEntryAssembly()?.GetName().Name;
            });

            services.PostConfigure<EfAuditOptions>(options => AuditInterceptorOptionsValidator.Validate(options));

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
