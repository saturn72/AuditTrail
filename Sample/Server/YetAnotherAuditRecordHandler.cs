using AuditTrail.Common;

namespace Server
{
    public class YetAnotherAuditRecordHandler : IAuditRecordHandler
    {
        private readonly ILogger<YetAnotherAuditRecordHandler> _logger;

        public YetAnotherAuditRecordHandler(ILogger<YetAnotherAuditRecordHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(IServiceProvider services, AuditRecord auditRecord, CancellationToken cancellationToken)
        {
            _logger.LogInformation("audit-trail entry was receieved!");
            return Task.CompletedTask;
        }
    }
}
