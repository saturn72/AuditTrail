using AuditTrail.Common;
using EfAudit.Common.Mappers;
using Shouldly;

namespace EfAudit.Tests.Mappers
{
    public class DefaultAuditRecordToAuditMessageMapperTests
    {
        [Theory]
        [MemberData(nameof(ReturnsEmptyStringOnInvaldAuditRecord_DATA))]
        public async Task ReturnsEmptyStringOnInvaldAuditRecord(AuditRecord record)
        {
            var m = new DefaultAuditRecordToAuditMessageMapper();
            var res = m.MapAsync(record);
            res.ShouldBeNull();
        }
        public static IEnumerable<object[]> ReturnsEmptyStringOnInvaldAuditRecord_DATA => new[]
        {
            new object []{ default( AuditRecord)},
            new object []{new AuditRecord()},
            new object []{new AuditRecord{Entities = new List<EntityAudit>()}},
        };
    }
}