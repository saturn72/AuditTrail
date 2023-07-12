
namespace EfAudit
{
    public class EfAuditOptions
    {
        private static readonly string[] OpenIdStandardClaims = new[] {
            "sub",
            "name",
            "given_name",
            "family_name",
            "middle_name",
            "nickname",
            "preferred_username",
            "profile",
            "picture",
            "website",
            "email",
            "email_verified",
            "gender",
            "birthdate",
            "zoneinfo",
            "locale",
            "phone_number",
            "phone_number_verified",
            "address",
            "updated_at",
        };
        internal const string Section = "efAudit";
        public string? Source { get; set; }
        public string[]? Claims { get; set; } = OpenIdStandardClaims;
    }
    public class AuditInterceptorOptionsValidator
    {
        public static void Validate(EfAuditOptions options)
        {
            if (options == null)
                Throw(nameof(options));

            if (options.Source.HasNoValue())
                Throw(nameof(options.Source));

            void Throw(string message) => throw new ArgumentNullException(message);
        }
    }
}



















