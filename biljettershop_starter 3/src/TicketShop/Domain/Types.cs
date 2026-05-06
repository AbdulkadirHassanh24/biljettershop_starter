namespace TicketShop.Domain;

public enum SeatType { Chair, Bench, LuxuryBox }
public enum SeatStatus { Available, Held, Sold }

public record SeatId(string Value)
{
    public override string ToString() => Value;
}

public class Seat
{
    public SeatId SeatId { get; init; }
    public SeatType Type { get; init; }
    public int Row { get; init; }
    public int Number { get; init; }

    public Seat(SeatId id, SeatType type, int row, int number)
    {
        SeatId = id; Type = type; Row = row; Number = number;
    }
}

public class Venue
{
    public string Name { get; set; } = "Arena";
    public string Material { get; set; } = "Tegel"; // eller Trä
    public bool EcoPaintCertified { get; set; } = true; // bänkar med miljövänlig färg

    public List<Seat> Seats { get; } = new();
}

public class Event
{
    public string EventId { get; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "Konsert";
    public DateTime StartsAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsIndoor { get; set; } = true;
    public bool IsFamilyEvent { get; set; } = false;
    public Venue Venue { get; init; }
    public List<Seat> Seats => Venue.Seats;

    private readonly List<Reservation> _reservations = new();
    private readonly List<Order> _orders = new();
    private TimeSpan _virtualClockOffset = TimeSpan.Zero;

    public Event(Venue venue) { Venue = venue; }

    public DateTime UtcNow() => DateTime.UtcNow + _virtualClockOffset;
    public void AdvanceClock(TimeSpan dt) => _virtualClockOffset += dt;

    public IEnumerable<Reservation> ActiveReservations() => _reservations.Where(r => !r.IsExpired(UtcNow()));
    public IEnumerable<Order> Orders() => _orders.AsEnumerable();

    public void AddReservation(Reservation r) => _reservations.Add(r);
    public void RemoveReservation(Reservation r) => _reservations.Remove(r);
    public void AddOrder(Order o) => _orders.Add(o);

    public SeatStatus GetSeatStatus(SeatId id)
    {
        if (_orders.Any(o => o.SeatIds.Contains(id.Value)))
            return SeatStatus.Sold;
        if (_reservations.Any(r => !r.IsExpired(UtcNow()) && r.SeatIds.Contains(id.Value)))
            return SeatStatus.Held;
        return SeatStatus.Available;
    }
}

public class Reservation
{
    public string ReservationId { get; } = Guid.NewGuid().ToString("N");
    public string BuyerEmail { get; init; }
    public List<string> SeatIds { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public decimal TotalAmount { get; init; }

    public List<Seat> Seats { get; init; } = new();

    public bool IsExpired(DateTime now) => now >= ExpiresAt;
    public Reservation(string email, IEnumerable<string> seatIds, DateTime createdAt, DateTime expiresAt, decimal amount, IEnumerable<Seat> seats)
    {
        BuyerEmail = email; SeatIds = seatIds.ToList(); CreatedAt = createdAt; ExpiresAt = expiresAt; TotalAmount = amount; Seats = seats.ToList();
    }
}

public class Order
{
    public string OrderId { get; } = Guid.NewGuid().ToString("N");
    public string BuyerEmail { get; init; }
    public List<string> SeatIds { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public string PaymentMethod { get; init; } = "Faktura"; // "Direkt"

    public Order(string email, IEnumerable<string> seatIds, decimal amount, string pay = "Faktura")
    {
        BuyerEmail = email; SeatIds = seatIds.ToList(); TotalAmount = amount; PaymentMethod = pay;
    }
}

public static class DemoData
{
    public static Venue CreateVenue()
    {
        var v = new Venue { Name = "Stora Arenan", Material = "Tegel", EcoPaintCertified = true };
        for (var i = 1; i <= 10; i++)
            v.Seats.Add(new Seat(new SeatId($"A-{i}"), SeatType.Chair, 1, i)); // Röda fällstolar (krav i uppgiften)
        for (var i = 1; i <= 10; i++)
            v.Seats.Add(new Seat(new SeatId($"B-{i}"), SeatType.Bench, 2, i)); // Bänkar (ekofärg antas)
        // Lyxloge för framtiden
        for (var i = 1; i <= 2; i++)
            v.Seats.Add(new Seat(new SeatId($"VIP-{i}"), SeatType.LuxuryBox, 99, i));
        return v;
    }

    public static Event CreateEvent(Venue v, bool isFamilyEvent = false)
    {
        return new Event(v) { Title = "Megashow", IsIndoor = true, IsFamilyEvent = isFamilyEvent };
    }
}
