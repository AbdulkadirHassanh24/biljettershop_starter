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
        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            throw new ArgumentException("Ogiltig e-postadress.");

        if (seatIds is null)
            throw new ArgumentException("Platser måste anges.");

        var now = evt.UtcNow();

        var ids = seatIds
            .Select(id => id.Trim())
            .ToList();

        if (ids.Count == 0)
            throw new ArgumentException("inga platser angivna.");

        if (ids.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Plats-Id får inte vara tomt.");

        if (ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() != ids.Count)
            throw new ArgumentException("samma plats får inte reserveras flera gånger.");

        _limit.EnsureAllowed(email, ids.Count, evt.IsFamilyEvent);

        var seats = ids
            .Select(id => evt.Seats.FirstOrDefault(s => s.SeatId.Value == id))
            .ToList();

        if (seats.Any(s => s is null))
            throw new InvalidOperationException("En eller flera platser finns inte.");

        if (seats.Any(s => evt.GetSeatStatus(s!.SeatId) != SeatStatus.Available))
            throw new InvalidOperationException("En eller flera platser är inte tillgängliga.");

        var amount = seats.Sum(s => _pricing.PriceFor(s!, evt));

        var reservation = new Reservation(email, ids, now, _expiry.ExpiresAt(now), amount, seats!);

        evt.AddReservation(reservation);

        return reservation;
    }

    private static bool IsValidEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        var dotIndex = email.LastIndexOf('.');

        return atIndex > 0 &&
               dotIndex > atIndex + 1 &&
               dotIndex < email.Length - 1;
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
