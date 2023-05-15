using AuditTrail.Common;
using EfAudit;
using Microsoft.Extensions.DependencyInjection;

namespace NServiceBusSink
{
    public sealed class CommandSink
    {
        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var messageSession = services.GetRequiredService<IMessageSession>();
            await messageSession.Send(record, cancellationToken);
        }
    }
}