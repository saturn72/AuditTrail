using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Server.NServiceBus;

public class AuditMessageHandler : IHandleMessages<PayloadMessage>
{
    private readonly IServiceProvider _services;
    private static Server.NServiceBus.PayloadMessage? _records;

    public AuditMessageHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task Handle(PayloadMessage eventMessage, IMessageHandlerContext context)
    {
        _records = eventMessage;
        return Task.CompletedTask;
    }

    public static object OnGet() => new JsonResult(_records ?? default);
}