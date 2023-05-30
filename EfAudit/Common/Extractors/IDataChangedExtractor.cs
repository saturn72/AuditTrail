namespace EfAudit.Common.Extractors
{
    public interface IAuditRecordToAuditMessageMapper
    {
        AuditMessage? Map(AuditRecord record);
    }
}
