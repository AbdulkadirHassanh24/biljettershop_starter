using TicketShop.Domain;
using TicketShop.Domain.Pricing;
using Xunit;

namespace TicketShop.Tests;

public class BasicPriceStrategyTests
{
    [Fact]
    public void PriceFor_Chair_ShouldReturn350()
    {
        var strategy = new BasicPriceStrategy();
        var evt = TestData.CreateEvent();
        var seat = new Seat(new SeatId("A-1"), SeatType.Chair, 1, 1);

        var price = strategy.PriceFor(seat, evt);

        Assert.Equal(350m, price);
    }

    [Fact]
    public void PriceFor_Bench_ShouldReturn200()
    {
        var strategy = new BasicPriceStrategy();
        var evt = TestData.CreateEvent();
        var seat = new Seat(new SeatId("B-1"), SeatType.Bench, 2, 1);

        var price = strategy.PriceFor(seat, evt);

        Assert.Equal(200m, price);
    }

    [Fact]
    public void PriceFor_LuxuryBox_ShouldReturn1500()
    {
        var strategy = new BasicPriceStrategy();
        var evt = TestData.CreateEvent();
        var seat = new Seat(new SeatId("VIP-1"), SeatType.LuxuryBox, 99, 1);

        var price = strategy.PriceFor(seat, evt);

        Assert.Equal(1500m, price);
    }
}