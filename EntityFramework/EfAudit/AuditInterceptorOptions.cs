
namespace EfAudit
{
    public class AuditInterceptorOptions
    {
        public string? ServiceName { get; set; }

        public void Validate(AuditInterceptorOptions options)
        {
            if (options == null)
                Throw(nameof(options));

            if (options.ServiceName.HasNoValue())
                Throw(nameof(options.ServiceName));

            void Throw(string message) => throw new ArgumentNullException(message);
        }
    }
}
