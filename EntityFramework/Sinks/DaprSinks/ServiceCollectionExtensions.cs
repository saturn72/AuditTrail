
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDaprSinks(this IServiceCollection services)
        {
            services.AddDaprClient();
            return services;
        }
    }
}
