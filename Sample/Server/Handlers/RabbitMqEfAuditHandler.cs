
using AuditTrail.Common;
using EasyNetQ;

namespace Server.Handlers
{
    public class RabbitMqEfAuditHandler : IAuditMessageHandler
    {
        private readonly IServiceProvider _services;

        public RabbitMqEfAuditHandler(IServiceProvider services)
        {
            _services = services;
        }
        public async Task Handle(IAuditMessageHandler.OutgoingMessage message, CancellationToken cancellationToken)
        {
            var bus = _services.GetRequiredService<IBus>();
            await bus.PubSub.PublishAsync(message, cancellationToken);
        }
    }
}