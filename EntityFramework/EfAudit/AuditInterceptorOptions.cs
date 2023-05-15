
namespace EfAudit
{
    public class AuditInterceptorOptions
    {
        internal const string Section = "efAudit";
        public string? Source { get; set; }
    }
    public class AuditInterceptorOptionsValidator
    {
        public static void Validate(AuditInterceptorOptions options)
        {
            if (options == null)
                Throw(nameof(options));

            if (options.Source.HasNoValue())
                Throw(nameof(options.Source));

            void Throw(string message) => throw new ArgumentNullException(message);
        }
    }
}
