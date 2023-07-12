using System.Text.Json;
using EfAudit.Common.Mappers;

namespace AuditTrail.Common
{
    public sealed class DefaultAuditMessageProcessor
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static async Task Handle(IServiceProvider services, AuditRecord auditRecord, CancellationToken cancellationToken)
        {
            if (auditRecord == null)
                throw new ArgumentNullException(nameof(auditRecord));

            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var logger = serviceProvider.GetRequiredService<ILogger<DefaultAuditMessageProcessor>>();
            logger.LogDebug($"Start processing {nameof(AuditRecord)} with {nameof(AuditRecord.Uuid)} = \'{auditRecord.Uuid}\'");

            var map = serviceProvider.GetRequiredService<IAuditRecordToAuditMessageMapper>();
            var am = await map.MapAsync(auditRecord, cancellationToken);
            if (am == default)
                return;

            object? transactionId = null, provider = null;

            _ = am.ProviderInfo != null &&
            am.ProviderInfo.TryGetValue("transactionId", out transactionId) &&
            am.ProviderInfo.TryGetValue("provider", out provider);

            var ce = serviceProvider.GetRequiredService<IAuditMessageSubjectBuilder>();
            var subject = await ce.BuildAsync(am.SubjectId, cancellationToken);

            var payload = new
            {
                error = am.Error,
                source = am.Source,
                subject = subject,
                traceId = am.TraceId,
                transaction = new
                {
                    id = transactionId,
                    startedOnUtc = am.Trail?.StartedOnUtc,
                    endedOnUtc = am.Trail?.EndedOnUtc,
                    provider,
                    entries = am.Trail?.Entries ?? Enumerable.Empty<object>(),
                }
            };

            var payloadJson = JsonSerializer.Serialize(payload, JsonSerializerOptions);
            var msg = new IAuditMessageHandler.OutgoingMessage
            {
                Version = am.Version,
                Key = "audit-trail",
                Payload = payloadJson,
            };

            var mh = serviceProvider.GetRequiredService<IAuditMessageHandler>();
            await mh.Handle(msg, cancellationToken);
        }
    }
    public interface IAuditMessageHandler
    {
        Task Handle(OutgoingMessage message, CancellationToken cancellationToken);
        public record OutgoingMessage
        {
            public string? Version { get; init; }
            public string? Key { get; init; }
            public string? Payload { get; init; }
        }
    }
}
