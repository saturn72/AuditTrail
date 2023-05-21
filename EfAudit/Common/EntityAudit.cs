namespace AuditTrail.Common
{
    public record EntityAudit
    {
        public object? PrimaryKeyValue { get; set; }
        public string? State { get; init; }
        public object? Value { get; set; }
        public string? TypeName { get; init; }
        public IEnumerable<ModifiedProperty>? ModifiedProperties { get; init; }
        public Guid Uuid { get; } = Guid.NewGuid();

        public record ModifiedProperty
        {
            public string? Name { get; init; }
            public Type? Type { get; init; }
            public object? OriginalValue { get; init; }
            public object? CurrentValue { get; init; }
        }
    }
}
