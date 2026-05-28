using Moq;
using TicketShop.Domain;
using TicketShop.Domain.Policies;
using TicketShop.Domain.Pricing;
using TicketShop.Services;
using Xunit;

namespace TicketShop.Tests;

public class OrderServiceTests
{
    [Fact]
    public void Confirm_WithValidReservation_ShouldCreateOrder()
    {
        var evt = TestData.CreateEvent();

        var priceMock = new Mock<IPriceStrategy>();
        priceMock
            .Setup(p => p.PriceFor(It.IsAny<Seat>(), evt))
            .Returns(100m);

        var reservationService = new ReservationService(
            priceMock.Object,
            new SeatLimitPolicy(5),
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        var reservation = reservationService.HoldSeats(
            evt,
            "buyer@test.com",
            new[] { "A-1" }
        );

        var orderService = new OrderService(
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        var order = orderService.Confirm(evt, reservation.ReservationId);

        Assert.Equal("buyer@test.com", order.BuyerEmail);
        Assert.Single(order.SeatIds);
        Assert.Equal(100m, order.TotalAmount);
        Assert.Equal(SeatStatus.Sold, evt.GetSeatStatus(new SeatId("A-1")));
    }

    [Fact]
    public void Confirm_WithInvalidReservation_ShouldThrowException()
    {
        var evt = TestData.CreateEvent();

        var orderService = new OrderService(
            new ReservationExpiryPolicy(TimeSpan.FromMinutes(10))
        );

        Assert.Throws<InvalidOperationException>(() =>
            orderService.Confirm(evt, "INVALID-ID")
        );
    }
}