namespace AuditTrail.Common
{
    public record AuditRecord
    {
        public Guid Uuid { get; } = Guid.NewGuid();
        public DateTime StartedOnUtc { get; } = DateTimeOffset.UtcNow.DateTime;
        public DateTime EndedOnUtc { get; set; }
        public bool? Success { get; set; }
        public string? SubjectId { get; set; }
        public Exception? Exception { get; set; }
        public IReadOnlyCollection<EntityAudit>? Entities { get; set; }
        public object? TraceId { get; set; }
        public Dictionary<string, object>? ProviderInfo { get; set; }

        public string? Source { get; set; }
    }
}
