namespace AuditTrail.Common
{
    public interface IAuditMessageSubjectBuilder
    {
        Task<string> BuildAsync(string? subjectId, CancellationToken cancellationToken);
    }
}
