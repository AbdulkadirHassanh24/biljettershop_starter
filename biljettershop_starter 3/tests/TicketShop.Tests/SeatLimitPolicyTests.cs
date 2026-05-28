using TicketShop.Domain.Policies;
using Xunit;

namespace TicketShop.Tests;

public class SeatLimitPolicyTests
{
    [Fact]
    public void EnsureAllowed_WithFiveSeats_ShouldNotThrow()
    {
        var policy = new SeatLimitPolicy(5);

        policy.EnsureAllowed("test@test.com", 5, false);
    }

    [Fact]
    public void EnsureAllowed_WithMoreThanFiveSeats_ShouldThrowException()
    {
        var policy = new SeatLimitPolicy(5);

        Assert.Throws<InvalidOperationException>(() =>
            policy.EnsureAllowed("test@test.com", 6, false)
        );
    }

    [Fact]
    public void EnsureAllowed_ForFamilyEvent_ShouldAllowMoreThanFiveSeats()
    {
        var policy = new SeatLimitPolicy(5);

        policy.EnsureAllowed("test@test.com", 10, true);
    }
}