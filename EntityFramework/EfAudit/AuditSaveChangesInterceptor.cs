using static EfAudit.EntityAudit;

namespace EfAudit
{
    public class AuditSaveChangesInterceptor : ISaveChangesInterceptor
    {
        private const string Added = "added";
        private readonly AuditInterceptorOptions _options;
        private readonly IServiceProvider _services;
        private readonly IEventBus _eventBus;
        private AuditRecord? _record;
        private readonly Dictionary<Guid, object> _trackedEntities = new();
        private readonly IReadOnlyDictionary<EntityState, string> _stateMap = new Dictionary<EntityState, string>
        {
            { EntityState.Added, Added},
            { EntityState.Modified, "modified" },
            { EntityState.Deleted, "deleted"},
            { EntityState.Unchanged, "unchanged"}
        };

        public AuditSaveChangesInterceptor(
            IServiceProvider services,
            AuditInterceptorOptions options)
        {
            _options = options;
            _services = services;
            _eventBus = _services.GetRequiredService<IEventBus>();
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            _record = CreateAuditRecord(eventData.Context);
            return ValueTask.FromResult(result);
        }

        public ValueTask<InterceptionResult<int>> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            _record = CreateAuditRecord(eventData.Context);
            return ValueTask.FromResult(result);
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (_record == default)
                throw new InvalidOperationException();

            _record.Success = true;
            _record.EndedOnUtc = DateTimeOffset.UtcNow.DateTime;

            _eventBus.PublishAsync(_record).Wait();
            return result;
        }
        public async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            foreach (var e in _record.Entities)
            {
                if (e.State == Added)
                    e.Value = _trackedEntities[e.Uuid].Clone();
            }
            _record.Success = true;
            _record.EndedOnUtc = DateTimeOffset.UtcNow.DateTime;

            await _eventBus.PublishAsync(_record);
            return result;
        }
        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            _record.Success = false;
            _record.Exception = eventData.Exception;
            var t = _eventBus.PublishAsync(_record);
            t.Wait();
        }

        public async Task SaveChangesFailedAsync(
            DbContextErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            _record.Success = false;
            _record.Exception = eventData.Exception;
            await _eventBus.PublishAsync(_record);
        }

        private AuditRecord CreateAuditRecord(DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            var record = new AuditRecord();
            var entries = context.ChangeTracker.Entries();
            var entities = new List<EntityAudit>();

            foreach (var entry in entries)
            {
                entities.Add(ToEntityAudit(entry));
            }
            record.Entities = entities;
            return record;
        }

        private EntityAudit ToEntityAudit(EntityEntry entry)
        {
            var modified = entry.State == EntityState.Modified ?
                getModifiedProperties() : null;

            var state = _stateMap[entry.State];
            var ea = new EntityAudit
            {
                PrimaryKeyValue = entry.Properties.First(p => p.Metadata.IsPrimaryKey()).OriginalValue,
                State = state,
                TypeName = entry.Metadata.ShortName(),
                Value = entry.Entity.Clone(),
                ModifiedProperties = modified ?? Array.Empty<ModifiedProperty>(),
            };


            _trackedEntities[ea.Uuid] = entry.Entity;

            return ea;
            IEnumerable<ModifiedProperty>? getModifiedProperties()
            {
                var m = entry.Properties.Where(p => p.IsModified && !p.CurrentValue.Equals(p.OriginalValue));
                return m?
                    .Select(c => new ModifiedProperty
                    {
                        Name = c.Metadata.Name,
                        CurrentValue = c.CurrentValue,
                        OriginalValue = c.OriginalValue,
                        Type = c.Metadata.ClrType,
                    }).ToList();
            }
        }
    }
}