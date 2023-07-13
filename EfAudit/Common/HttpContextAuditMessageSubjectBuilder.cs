using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AuditTrail.Common
{
    public class HttpContextAuditMessageSubjectBuilder : IAuditMessageSubjectBuilder
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IOptionsMonitor<EfAuditOptions> _options;

        public HttpContextAuditMessageSubjectBuilder(IHttpContextAccessor accessor,
        IOptionsMonitor<EfAuditOptions> options)
        {
            _accessor = accessor;
            _options = options;
        }

        public Task<string> BuildAsync(string? subjectId, CancellationToken cancellationToken)
        {
            var user = _accessor.HttpContext?.User;
            if (user == null || user.Claims == null || !user.Claims.Any())
                return Task.FromResult(string.Empty);

            var sb = new StringBuilder();
            var ctf = _options.CurrentValue.Claims;

            foreach (var c in ctf)
            {
                var mcs = user.FindAll(x => x.Type.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (mcs == null || !mcs.Any())
                    continue;

                foreach (var mc in mcs)
                    sb.Append($"{c}={mc.Value};");
            }
            return Task.FromResult(sb.ToString());
        }
    }
}
