namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static void UseEfAudit(this IHost host)
        {
            var eb = host.Services.GetRequiredService<IEventBus>();
            var arhType = typeof(IAuditRecordHandler);
            using var scope = host.Services.CreateScope();

            //these were registered as `IAuditRecordHandler` type
            var regAsARH = scope.ServiceProvider.GetServices<IAuditRecordHandler>().ToList();
            foreach (var h in regAsARH)
            {
                var hw = new HandlerWarpper(h.GetType(), true);
                eb.Subscribe(hw.Handle, h.Name);
            }

            //these were registered explicitly
            var exp = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && arhType.IsAssignableFrom(t))
                .Where(t => regAsARH.Any(x => x.GetType() != t)).ToList();

            foreach (var expType in exp)
            {
                var hw = new HandlerWarpper(expType, false);
                eb.Subscribe(hw.Handle);
            }
        }
    }

    public sealed class HandlerWarpper
    {
        private Type _type;
        private readonly Func<IServiceProvider, IAuditRecordHandler> _resolver;

        public HandlerWarpper(Type type, bool registeredAsAuditRecordHandler)
        {
            this._type = type;
            _resolver = registeredAsAuditRecordHandler ?
                 s => s.GetServices<IAuditRecordHandler>().First(x => x.GetType() == _type) as IAuditRecordHandler :
                 s => s.GetRequiredService(_type) as IAuditRecordHandler;

        }

        internal Task Handle(IServiceProvider services, AuditRecord record, CancellationToken cancellationToken)
        {
            var h = _resolver(services);
            return h!.Handle(services, record, cancellationToken);
        }
    }
}
