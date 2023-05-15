namespace EfAudit.Extractors
{
    public class DefaultDataChangedExtractor : IDataChangedExtractor
    {
        public AuditMessage? Extract(AuditRecord record)
        {
            if (record == null || record.Entities == null || !record.Entities.Any())
                return default;

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
                    id = e.PrimaryKeyValue,
                    action = e.State,
                    type = e.TypeName,
                    value = e.Value,
                    diff
                };
                l.Add(o);
            }
            if (!l.Any())
                return default;

            return new AuditMessage
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
        }
    }
}
