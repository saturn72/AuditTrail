
using EfAudit.Extractors;
using System.Text.Json;

namespace Server.NServiceBus
{
    public class NServiceBusEfHandler
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var messageSession = services.GetRequiredService<IMessageSession>();
            var extractor = services.GetRequiredService<IDataChangedExtractor>();
            var msg = extractor.Extract(record);

            if (msg == default)
                throw new ArgumentNullException(nameof(msg));

            var payloadObject = new
            {
                source = msg.Source,
                subjectId = msg.SubjectId,
                error = msg.Error,
                transaction = new
                {
                    id = msg.TransactionId,
                    trail = msg?.Trail?.Entries ?? Enumerable.Empty<object>(),
                },
            };
            var payload = JsonSerializer.Serialize(payloadObject, JsonSerializerOptions);

            var message = new PayloadMessage
            {
                Version = msg.Version,
                Key = "audit-trail",
                Payload = payload
            };

            await messageSession.Send(message, cancellationToken);
        }
    }

    public class PayloadMessage : IMessage
    {
        public string? Version { get; set; }
        public string? Key { get; set; }
        public string? Payload { get; set; }
    }
}