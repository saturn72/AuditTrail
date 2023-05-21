namespace AuditTrail.Common
{
    public interface IAuditRecordHandler
    {
        string? Name { get => this.GetType().Name; }
        Task Handle(IServiceProvider services, AuditRecord auditRecord, CancellationToken cancellationToken);
    }
}
