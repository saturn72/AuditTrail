
using Microsoft.Extensions.Logging;

namespace EfAudit
{
    public class DefaultEventBus : IEventBus
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DefaultEventBus> _logger;
        private readonly List<HandlerWrapper> _handlerWrappers = new();

        public DefaultEventBus(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<DefaultEventBus> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        public async Task PublishAsync(AuditRecord audit)
        {
            _logger.LogDebug("Start publishing");

            var h = _handlerWrappers.Select(async s =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await s.Execute(scope.ServiceProvider, audit, default);
            });
            await Task.WhenAll(h);
            _logger.LogDebug("End publishing");
        }
        public void Subscribe(Func<IServiceProvider, AuditRecord, CancellationToken, Task> handler, string? key = null)
        {
            var n = key ?? Guid.NewGuid().ToString();

            if (_handlerWrappers.Any(h => h.Key == n))
                throw new ArgumentException($"Handler with {nameof(key)} = \'{key}\' already exists", nameof(key));

            _handlerWrappers.Add(new HandlerWrapper(handler, n, _logger));
            _logger.LogDebug($"handler named: {n} was added");
        }

        #region nested classes

        private class HandlerWrapper
        {
            private readonly ILogger _logger;
            public readonly string Key;
            public readonly Func<IServiceProvider, AuditRecord, CancellationToken, Task> Handler;
            public HandlerWrapper(
                Func<IServiceProvider, AuditRecord, CancellationToken, Task> handler,
                string key,
                ILogger logger)
            {
                _logger = logger;
                Key = key;
                Handler = handler;
            }
            public async Task Execute(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
            {
                _logger.LogDebug($"Start execution of \'{Key}\'");
                await Handler(services, record, cancellationToken);
                _logger.LogDebug($"Execution of \'{Key}\' ended");
            }
        }
        #endregion
    }
}
