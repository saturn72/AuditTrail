using EfAudit;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("audit")]
    public class AuditTrailController : ControllerBase
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


        public static Task AddRecords(IServiceProvider services, AuditRecord auditRecord, CancellationToken cancellationToken)
        {
            if (auditRecord == null || auditRecord.Entities == null || !auditRecord.Entities.Any())
                return Task.CompletedTask;

            var l = new List<object>();
            foreach (var e in auditRecord.Entities)
            {
                var diff = e.ModifiedProperties.Select(x => new
                {
                    attributeName = x.Name,
                    currentValue = x.CurrentValue,
                    previousValue = x.OriginalValue,
                }).ToList();

                var o = new
                {
                    action = e.State,
                    type = e.TypeName,
                    value = e.Value,
                    diff
                };
                l.Add(o);
            }
            if (!l.Any())
                return Task.CompletedTask;

            var r = new
            {
                version = "v1",
                transaction = new
                {
                    success = auditRecord.Success,
                    id = auditRecord.Uuid,
                    startedOnUtc = auditRecord.StartedOnUtc,
                    endedOnUtc = auditRecord.EndedOnUtc,
                },
                error = auditRecord.Exception?.InnerException?.Message,
                trail = l
            };

            _records.Add(r);
            return Task.CompletedTask;
        }
    }
}
