namespace EfAudit.Common.Extractors
{
    public interface IAuditRecordToAuditMessageMapper
    {
        Task<AuditMessage?> MapAsync(AuditRecord record);
    }
}
