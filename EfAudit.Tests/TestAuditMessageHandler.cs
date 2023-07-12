using AuditTrail.Common;
namespace EfAudit.Tests
{
    public class TestAuditMessageHandler : IAuditMessageHandler
    {
        public Task Handle(IAuditMessageHandler.OutgoingMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}