
using EfAudit.Extractors;
using System.Text.Json;

namespace Server.NServiceBus
{
    public class NServiceBusEfHandler
    {
        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var messageSession = services.GetRequiredService<IMessageSession>();
            var extractor = services.GetRequiredService<IDataChangedExtractor>();
            var msg = extractor.Extract(record);

            if (msg == default)
                throw new ArgumentNullException(nameof(msg));

            await messageSession.Send<IAuditRecordMessage>(message =>
                {
                    message.Version = msg.Version;
                    message.Error = msg.Error;
                    message.Transaction = msg.Transaction;
                    message.Trail = msg.Trail.Select(x => JsonSerializer.Serialize(x)).ToList();
                },
                cancellationToken);
        }
    }

    public interface IAuditRecordMessage : IMessage
    {
        string? Version { get; set; }
        string? Error { get; set; }
        object? Transaction { get; set; }
        List<string>? Trail { get; set; }
    }
}