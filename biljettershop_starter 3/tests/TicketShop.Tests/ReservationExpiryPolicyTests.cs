using TicketShop.Domain.Policies;
using Xunit;

namespace TicketShop.Tests;

public class ReservationExpiryPolicyTests
{
    [Fact]
    public void ExpiresAt_ShouldAddTenMinutes()
    {
        var policy = new ReservationExpiryPolicy(
            TimeSpan.FromMinutes(10)
        );

        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0);

        var expiresAt = policy.ExpiresAt(createdAt);

        Assert.Equal(
            new DateTime(2026, 1, 1, 12, 10, 0),
            expiresAt
        );
    }

    [Fact]
    public void ExpiresAt_ShouldAddThirtyMinutes()
    {
        var policy = new ReservationExpiryPolicy(
            TimeSpan.FromMinutes(30)
        );

        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0);

        var expiresAt = policy.ExpiresAt(createdAt);

        Assert.Equal(
            new DateTime(2026, 1, 1, 12, 30, 0),
            expiresAt
        );
    }
}