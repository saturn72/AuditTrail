
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static string? GetCurrentTransactionId(this DbContext context)
            => context?.Database?.CurrentTransaction?.TransactionId.ToString() ?? default;
    }
}
