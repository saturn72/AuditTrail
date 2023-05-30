using static AuditTrail.Common.AuditTrailRecord;

namespace EfAudit.Common.Extractors
{
    public class DefaultAuditRecordToAuditMessageMapper : IAuditRecordToAuditMessageMapper
    {
        public AuditMessage? Map(AuditRecord record)
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

            return new AuditMessage
            {
                Version = "v1",
                SubjectId = record.SubjectId,
                Source = record.Source,
                ProviderInfo = new Dictionary<string, object>
                {
                    { "transactionId", record.TraceId }
                },
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

            IEnumerable<AuditTrailRecordEntryDiff> getDiff(EntityAudit entry)
            {
                return entry.ModifiedProperties?.Select(x => new AuditTrailRecordEntryDiff
                {
                    AttributeName = x.Name,
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
