namespace EfAudit.Extractors
{
    public interface IDataChangedExtractor
    {
        AuditMessage? Extract(AuditRecord record);
    }
}
