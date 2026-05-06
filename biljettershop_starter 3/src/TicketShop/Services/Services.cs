using TicketShop.Domain;
using TicketShop.Domain.Policies;
using TicketShop.Domain.Pricing;

namespace TicketShop.Services;

public class ReservationService
{
    private readonly IPriceStrategy _pricing;
    private readonly ISeatLimitPolicy _limit;
    private readonly IReservationExpiryPolicy _expiry;

    public ReservationService(IPriceStrategy pricing, ISeatLimitPolicy limit, IReservationExpiryPolicy expiry)
    { _pricing = pricing; _limit = limit; _expiry = expiry; }

    public Reservation HoldSeats(Event evt, string email, IEnumerable<string> seatIds)
    {
        var now = evt.UtcNow();
        var ids = seatIds.ToList();
        if (ids.Count == 0) throw new ArgumentException("Inga platser angivna.");

        _limit.EnsureAllowed(email, ids.Count, evt.IsFamilyEvent);

        var seats = ids.Select(id => evt.Seats.FirstOrDefault(s => s.SeatId.Value == id)).ToList();
        if (seats.Any(s => s is null))
            throw new InvalidOperationException("En eller flera platser finns inte.");
        if (seats.Any(s => evt.GetSeatStatus(s!.SeatId) != SeatStatus.Available))
            throw new InvalidOperationException("En eller flera platser är inte tillgängliga.");

        var amount = seats.Sum(s => _pricing.PriceFor(s!, evt));
        var r = new Reservation(email, ids, now, _expiry.ExpiresAt(now), amount, seats!);
        evt.AddReservation(r);
        return r;
    }
}

public class OrderService
{
    private readonly IReservationExpiryPolicy _expiry;
    public OrderService(IReservationExpiryPolicy expiry) => _expiry = expiry;

    public Order Confirm(Event evt, string reservationId, string paymentMethod = "Faktura")
    {
        var now = evt.UtcNow();
        var r = evt.ActiveReservations().FirstOrDefault(x => x.ReservationId == reservationId);
        if (r is null) throw new InvalidOperationException("Ogiltig eller utgången reservation.");

        var order = new Order(r.BuyerEmail, r.SeatIds, r.TotalAmount, paymentMethod);
        evt.AddOrder(order);

        var res = evt.ActiveReservations().FirstOrDefault(x => x.ReservationId == reservationId);
        if (res != null) evt.RemoveReservation(res);
        return order;
    }
}
