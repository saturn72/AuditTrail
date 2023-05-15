namespace AuditTrail.Common
{
    public class AuditMessage
    {
        public string? Version { get; set; }
        public string? Error { get; set; }
        public object? Transaction { get; set; }
        public List<object>? Trail { get; set; }
    }
}
