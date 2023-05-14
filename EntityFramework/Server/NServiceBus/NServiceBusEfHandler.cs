using EfAudit;

namespace Server.NServiceBus
{
    public class NServiceBusEfHandler
    {
        public static async Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var messageSession = services.GetRequiredService<IMessageSession>();

            if (record == null || record.Entities == null || !record.Entities.Any())
                return;

            var l = new List<object>();
            foreach (var e in record.Entities)
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
                return;

            var msg = new AuditMessage
            {
                Version = "v1",
                Transaction = new
                {
                    success = record.Success,
                    id = record.Uuid,
                    startedOnUtc = record.StartedOnUtc,
                    endedOnUtc = record.EndedOnUtc,
                },
                Error = record.Exception?.InnerException?.Message,
                Trail = l
            };

            await messageSession.Send(msg, cancellationToken);
        }
    }
}
