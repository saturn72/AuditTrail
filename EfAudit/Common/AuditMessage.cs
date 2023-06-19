namespace AuditTrail.Common
{
    public class AuditMessage
    {
        public string? Version { get; set; }
        public string? Source { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, object?>? ProviderInfo { get; set; }
        public AuditTrailRecord? Trail { get; set; }
        public string? SubjectId { get; set; }
        public object? TraceId { get; set; }
    }
    public class AuditTrailRecord
    {
        public Guid Id { get; set; }

        public bool Success { get; set; }
        public DateTime StartedOnUtc { get; set; }
        public DateTime EndedOnUtc { get; set; }
        public IEnumerable<AuditTrailRecordEntry>? Entries { get; set; }

        public class AuditTrailRecordEntry
        {
            public object? EntityId { get; set; }
            public string? Action { get; set; }
            public string? EntityType { get; set; }
            public object? EntityValue { get; set; }
            public IEnumerable<AuditTrailRecordEntryDiff>? Diff { get; set; }
        }
        public class AuditTrailRecordEntryDiff
        {
            public string? AttributeName { get; set; }
            public string? AttributeType { get; set; }
            public object? CurrentValue { get; set; }
            public object? PreviousValue { get; set; }
        }
    }
}
