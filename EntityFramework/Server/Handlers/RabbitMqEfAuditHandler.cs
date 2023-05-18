
using EasyNetQ;
using EfAudit.Extractors;
using System.Text.Json;

namespace Server.Handlers
{
    public class RabbitMqEfAuditHandler
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var bus = services.GetRequiredService<IBus>();
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
                    id = msg.ProviderInfo,
                    trail = msg?.Trail?.Entries ?? Enumerable.Empty<object>(),
                },
            };
            var payload = System.Text.Json.JsonSerializer.Serialize(payloadObject, JsonSerializerOptions);

            var message = new PayloadedMessage
            {
                Version = msg.Version,
                Key = "audit-trail",
                Payload = payload
            };

            await bus.PubSub.PublishAsync(message, cancellationToken);
        }
    }

    public class PayloadedMessage
    {
        public string? Version { get; set; }
        public string? Key { get; set; }
        public string? Payload { get; set; }
    }
}