using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static AuditTrail.Common.EntityAudit;

namespace EfAudit
{
    public class AuditSaveChangesInterceptor : ISaveChangesInterceptor
    {
        private readonly IOptionsMonitor<AuditInterceptorOptions> _options;
        private readonly IEventBus _eventBus;
        private readonly IHttpContextAccessor _accessor;
        private AuditRecord? _record;
        private readonly Dictionary<Guid, EntityEntry> _trackedEntities = new();
        private static readonly IReadOnlyDictionary<EntityState, string> StateToStringMap = new Dictionary<EntityState, string>
        {
            { EntityState.Added, "added"},
            { EntityState.Modified, "modified" },
            { EntityState.Deleted, "deleted"},
        };
        private static readonly string Added = StateToStringMap[EntityState.Added];
        private static readonly IEnumerable<EntityState> MonitoredStates = StateToStringMap.Keys;


        public AuditSaveChangesInterceptor(
            IEventBus eventBus,
            IHttpContextAccessor accessor,
            IOptionsMonitor<AuditInterceptorOptions> options)
        {
            _options = options;
            _eventBus = eventBus;
            _accessor = accessor;
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            _record = CreateAuditRecord(eventData.Context);
            return ValueTask.FromResult(result);
        }
        public InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            _record = CreateAuditRecord(eventData.Context);
            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            OnSavedChanges(eventData.Context).Wait();
            return result;
        }

        public async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            await OnSavedChanges(eventData.Context);
            return result;
        }

        private async Task OnSavedChanges(DbContext? context)
        {
            if (_record == default)
                throw new InvalidOperationException();

            _record.TransactionId = context.GetCurrentTransactionId();
            foreach (var e in _record.Entities.Where(x => x.State == Added))
            {
                var te = _trackedEntities[e.Uuid];
                e.PrimaryKeyValue = te.Properties.First(p => p.Metadata.IsPrimaryKey()).CurrentValue;
                e.Value = te.Entity.Clone();
            }
            _record.Success = true;
            _record.EndedOnUtc = DateTimeOffset.UtcNow.DateTime;

            await _eventBus.PublishAsync(_record);
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

            var record = new AuditRecord
            {
                SubjectId = _accessor.GetSubjectId(),
                Source = _options.CurrentValue.Source
            };

            var entries = context.ChangeTracker.Entries();
            var modifiedOrUpdated = entries.Where(x => MonitoredStates.Contains(x.State)).ToList();
            var entities = new List<EntityAudit>();

            foreach (var entry in modifiedOrUpdated)
                entities.Add(ToEntityAudit(entry));

            record.Entities = entities;
            return record;
        }

        private EntityAudit ToEntityAudit(EntityEntry entry)
        {
            var modified = entry.State == EntityState.Modified ?
                getModifiedProperties() : null;

            var state = StateToStringMap[entry.State];

            var ea = new EntityAudit
            {
                PrimaryKeyValue = entry.Properties.First(p => p.Metadata.IsPrimaryKey()).OriginalValue,
                State = state,
                TypeName = entry.Metadata.ShortName(),
                Value = entry.Entity.Clone(),
                ModifiedProperties = modified ?? Array.Empty<ModifiedProperty>(),
            };
            _trackedEntities[ea.Uuid] = entry;

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