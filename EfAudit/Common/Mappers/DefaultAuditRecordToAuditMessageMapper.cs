using static AuditTrail.Common.AuditTrailRecord;

namespace EfAudit.Common.Mappers
{
    public class DefaultAuditRecordToAuditMessageMapper : IAuditRecordToAuditMessageMapper
    {
        public Task<AuditMessage?> MapAsync(AuditRecord record, CancellationToken cancellationToken = default)
        {
            if (record == null || record.Entities == null || !record.Entities.Any())
                return default;

            var atre = new List<AuditTrailRecordEntry>();
            foreach (var e in record.Entities)
            {
                var diff = getDiff(e);
                var o = getAuditTrailRecordEntry(e, diff);
                atre.Add(o);
            }
            if (!atre.Any())
                return default;

            var msg = new AuditMessage
            {
                Version = "v1",
                SubjectId = record.SubjectId,
                Source = record.Source,
                ProviderInfo = record.ProviderInfo,
                TraceId = record.TraceId,
                Trail = new AuditTrailRecord
                {
                    Success = record.Success.Value,
                    Id = record.Uuid,
                    StartedOnUtc = record.StartedOnUtc,
                    EndedOnUtc = record.EndedOnUtc,
                    Entries = atre
                },
                Error = record.Exception?.InnerException?.Message,
            };

            return Task.FromResult(msg);

            IEnumerable<AuditTrailRecordEntryDiff> getDiff(EntityAudit entry)
            {
                return entry.ModifiedProperties?.Select(x => new AuditTrailRecordEntryDiff
                {
                    AttributeName = x.Name,
                    AttributeType = x.Type.Name.ToLower(),
                    CurrentValue = x.CurrentValue,
                    PreviousValue = x.OriginalValue,
                })?.ToList()
                ?? Enumerable.Empty<AuditTrailRecordEntryDiff>();
            }
            AuditTrailRecordEntry getAuditTrailRecordEntry(EntityAudit entry, IEnumerable<AuditTrailRecordEntryDiff> diff)
            {
                return new AuditTrailRecordEntry
                {
                    EntityId = entry.PrimaryKeyValue,
                    Action = entry.State,
                    EntityType = entry.TypeName,
                    EntityValue = entry.Value,
                    Diff = diff,
                };
            }

        }
    }
}
