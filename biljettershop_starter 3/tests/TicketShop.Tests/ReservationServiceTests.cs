using Moq;
using TicketShop.Domain;
using TicketShop.Domain.Policies;
using TicketShop.Domain.Pricing;
using TicketShop.Services;
using Xunit;

namespace TicketShop.Tests;

public class ReservationServiceTests
{
    [Fact]
    public void HoldSeats_WithNoSeats_ShouldThrowArgumentException()
    {
        var evt = TestData.CreateEvent();
        var priceMock = new Mock<IPriceStrategy>();

        var service = new ReservationService(
            priceMock.Object,
            new SeatLimitPolicy(5),
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        Assert.Throws<ArgumentException>(() =>
            service.HoldSeats(evt, "test@test.com", Array.Empty<string>())
        );
    }

    [Fact]
    public void HoldSeats_WithInvalidSeat_ShouldThrowInvalidOperationException()
    {
        var evt = TestData.CreateEvent();
        var priceMock = new Mock<IPriceStrategy>();

        var service = new ReservationService(
            priceMock.Object,
            new SeatLimitPolicy(5),
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        Assert.Throws<InvalidOperationException>(() =>
            service.HoldSeats(evt, "test@test.com", new[] { "X-99" })
        );
    }

    [Fact]
    public void HoldSeats_WithValidSeat_ShouldCreateReservation()
    {
        var evt = TestData.CreateEvent();

        var priceMock = new Mock<IPriceStrategy>();
        priceMock
            .Setup(p => p.PriceFor(It.IsAny<Seat>(), evt))
            .Returns(100m);

        var service = new ReservationService(
            priceMock.Object,
            new SeatLimitPolicy(5),
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        var reservation = service.HoldSeats(evt, "test@test.com", new[] { "A-1" });

        Assert.Equal("test@test.com", reservation.BuyerEmail);
        Assert.Single(reservation.SeatIds);
        Assert.Equal(100m, reservation.TotalAmount);
        Assert.Equal(SeatStatus.Held, evt.GetSeatStatus(new SeatId("A-1")));
    }
}