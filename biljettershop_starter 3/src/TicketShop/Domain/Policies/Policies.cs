namespace TicketShop.Domain.Policies;

public interface ISeatLimitPolicy
{
    void EnsureAllowed(string buyerEmail, int seatsRequested, bool isFamilyEvent);
}

public class SeatLimitPolicy : ISeatLimitPolicy
{
    private readonly int _maxPerPerson;
    public SeatLimitPolicy(int maxPerPerson) => _maxPerPerson = maxPerPerson;

    public void EnsureAllowed(string buyerEmail, int seatsRequested, bool isFamilyEvent)
    {
        if (isFamilyEvent) return;
        if (seatsRequested > _maxPerPerson)
            throw new InvalidOperationException($"Max {_maxPerPerson} biljetter per person.");
    }
}

public interface IReservationExpiryPolicy
{
    DateTime ExpiresAt(DateTime createdAt);
}

public class ReservationExpiryPolicy : IReservationExpiryPolicy
{
    private readonly TimeSpan _holdDuration;
    public ReservationExpiryPolicy(TimeSpan holdDuration) => _holdDuration = holdDuration;
    public DateTime ExpiresAt(DateTime createdAt) => createdAt + _holdDuration;
}
