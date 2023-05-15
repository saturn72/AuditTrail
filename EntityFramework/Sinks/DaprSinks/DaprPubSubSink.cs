using AuditTrail.Common;
using Dapr.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DaprSinks
{
    public class DaprPubSubSink
    {
        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var daprClient = services.GetRequiredService<DaprClient>();
            await daprClient.PublishEventAsync(
                "auditpubsub",
                "audittrail",
                record,
                cancellationToken);

            //var daprClient = new DaprClientBuilder().Build()
            //await daprClient.PublishEventAsync("pubsub", "newOrder", record);
        }
    }
}