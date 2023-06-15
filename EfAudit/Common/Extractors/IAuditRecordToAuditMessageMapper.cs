namespace EfAudit.Common.Extractors
{
    public interface IAuditRecordToAuditMessageMapper
    {
        Task<AuditMessage?> Map(AuditRecord record);
    }
}
