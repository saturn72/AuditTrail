using AuditTrail.Common;
using EfAudit.Common.Extractors;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("audit")]
    public class AuditTrailController : ControllerBase, IAuditRecordHandler
    {
        private static readonly List<object> _records = new();

        /// <summary>
        /// Gets all audittrail records
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllRecords()
        {
            return Ok(_records);
        }

        [NonAction]
        public Task Handle(IServiceProvider services, AuditRecord auditRecord, CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();

            var e = scope.ServiceProvider.GetRequiredService<IDataChangedExtractor>();
            var r = e.Extract(auditRecord);

            if (r != null)
                _records.Add(r);

            return Task.CompletedTask;
        }
    }
}
