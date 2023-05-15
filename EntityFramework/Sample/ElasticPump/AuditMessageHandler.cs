using AuditTrail.Common;
using Microsoft.AspNetCore.Mvc;
using Nest;
using NServiceBus;
using Server.NServiceBus;
using System.Text.Json;

public class AuditMessageHandler : IHandleMessages<IAuditRecordMessage>
{
    private readonly IServiceProvider _services;
    private static IAuditRecordMessage? _records ;

    public AuditMessageHandler(IServiceProvider services)
    {
        _services = services;
    }

    public Task Handle(IAuditRecordMessage eventMessage, IMessageHandlerContext context)
    {
        _records = eventMessage;
        return Task.CompletedTask;
    }

    public static object OnGet() => new JsonResult(_records ?? default);
}