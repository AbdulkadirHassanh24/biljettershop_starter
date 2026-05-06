using TicketShop.Domain;

namespace TicketShop.Domain.Pricing;

public interface IPriceStrategy
{
    decimal PriceFor(Seat seat, Event evt);
    decimal Total(IEnumerable<Seat> seats, Event evt) => seats.Sum(s => PriceFor(s, evt));
}

public class BasicPriceStrategy : IPriceStrategy
{
    public decimal PriceFor(Seat seat, Event evt)
    {
        return seat.Type switch
        {
            SeatType.Chair => 350m,  // dyrare stolar
            SeatType.Bench => 200m,
            SeatType.LuxuryBox => 1500m, // framtida premium
            _ => 250m
        };
    }
}
