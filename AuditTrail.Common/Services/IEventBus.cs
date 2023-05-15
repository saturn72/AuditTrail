namespace AuditTrail.Common.Services
{
    public interface IEventBus
    {
        void Subscribe(Func<IServiceProvider, AuditRecord, CancellationToken, Task> handler, string? handlerName = null);
        Task PublishAsync(AuditRecord audit);
    }
}
