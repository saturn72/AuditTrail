namespace Server.NServiceBus
{
    public class AuditMessage : IMessage
    {
        public string? Version { get; set; }
        public string? Error { get; set; }
        public object? Transaction { get; set; }
        public List<object>? Trail { get; set; }
    }
}
