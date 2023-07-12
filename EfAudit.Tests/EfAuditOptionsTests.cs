using Shouldly;

namespace EfAudit.Tests
{
    public class EfAuditOptionsTests
    {
        [Fact]
        public void EfAuditOptions_TestDefaultValues()
        {
            var exp = new[] {
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
            var c = new EfAuditOptions().Claims;

            c.Count().ShouldBe(exp.Length);
            c.ShouldAllBe(s => exp.Contains(s));
        }
    }
}