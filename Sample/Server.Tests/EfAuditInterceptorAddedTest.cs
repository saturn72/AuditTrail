using EfAudit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shouldly;
using Xunit;

namespace Server.Tests
{
    public class EfAuditInterceptorAddedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EfAuditInterceptorAddedTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void ValidatesThatEfSaveChangesInterceptorWasAddedOnConfig()
        {
            _factory.Services.ValidateEfAudit();
        }
    }
}