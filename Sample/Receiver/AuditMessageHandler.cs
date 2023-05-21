using Microsoft.AspNetCore.Mvc;
using Server.Handlers;

public class AuditMessageHandler
{
    private static PayloadedMessage? _records;

    public Task Handle(PayloadedMessage auditMessage)
    {
        //elastic client placed here
        _records = auditMessage;
        return Task.CompletedTask;
    }

    public static object OnGet() => new JsonResult(_records ?? default);
}