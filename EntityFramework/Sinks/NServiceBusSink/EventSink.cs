using EfAudit;
using Microsoft.Extensions.DependencyInjection;

namespace NServiceBusSink
{
    public sealed class EventSink
    {
        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var messageSession = services.GetRequiredService<IMessageSession>();
            await messageSession.Publish(record, cancellationToken);
        }
    }
}