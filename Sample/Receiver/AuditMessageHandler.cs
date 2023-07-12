using AuditTrail.Common;
using Microsoft.AspNetCore.Mvc;
using Server.Handlers;

public class AuditMessageHandler
{
    private static IAuditMessageHandler.OutgoingMessage? _records;

    public Task Handle(IAuditMessageHandler.OutgoingMessage auditMessage)
    {
        //elastic client placed here
        _records = auditMessage;
        return Task.CompletedTask;
    }

    public static object OnGet() => new JsonResult(_records ?? default);
}