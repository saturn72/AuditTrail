namespace EfAudit.Common.Mappers
{
    public interface IAuditRecordToAuditMessageMapper
    {
        Task<AuditMessage?> MapAsync(AuditRecord record, CancellationToken cancellationToken = default);
    }
}
