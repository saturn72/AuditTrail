using Nest;
using NServiceBus;
using Server.NServiceBus;

public class AuditMessageHandler : IHandleMessages<AuditMessage>
{
    private readonly IServiceProvider _services;

    public AuditMessageHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task Handle(AuditMessage eventMessage, IMessageHandlerContext context)
    {
        using var scope = _services.CreateScope();
        var elastic = scope.ServiceProvider.GetRequiredService<ElasticClient>();
        
        //push to elastic
        throw new NotImplementedException();
    }
}