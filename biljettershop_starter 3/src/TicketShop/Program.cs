using TicketShop.Domain;
using TicketShop.Domain.Policies;
using TicketShop.Domain.Pricing;
using TicketShop.Services;

Console.WriteLine("=== Biljettshoppen — Iteration 1 ===");

var venue = DemoData.CreateVenue();
var evt = DemoData.CreateEvent(venue, isFamilyEvent: false);
var pricing = new BasicPriceStrategy();
var seatLimit = new SeatLimitPolicy(maxPerPerson: 5);
var expiry = new ReservationExpiryPolicy(TimeSpan.FromMinutes(10));
var resService = new ReservationService(pricing, seatLimit, expiry);
var orderService = new OrderService(expiry);

while (true)
{
    Console.WriteLine("\n1) Lista platser  2) Reservera  3) Bekräfta köp  4) Visa aktiva reservationer  5) Simulera utgången reservation  6) Avsluta");
    Console.Write("> ");
    var cmd = Console.ReadLine();
    if (cmd == "1")
    {
        foreach (var s in evt.Seats.OrderBy(s => s.Row).ThenBy(s => s.Number))
        {
            Console.WriteLine($"{s.SeatId} | {s.Type} | Row {s.Row}, No {s.Number} | Status: {evt.GetSeatStatus(s.SeatId)}");
        }
    }
    else if (cmd == "2")
    {
        Console.Write("Köparens e-post: ");
        var email = Console.ReadLine();

        if (!IsValidEmail(email))
        {
            Console.WriteLine("Fel: Ogiltig e-postadress.");
            continue;
        }

        Console.Write("Ange seatIds separerade med mellanslag (ex: A-1 A-2): ");
        var ids = (Console.ReadLine() ?? "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.ToUpperInvariant())
            .ToArray();

        try
        {
            var result = resService.HoldSeats(evt, email!, ids);
            Console.WriteLine($"Reserverade {result.Seats.Count} platser för {email}. ReservationId: {result.ReservationId}. Giltig t.o.m. {result.ExpiresAt:HH:mm:ss}.");
            Console.WriteLine($"Belopp: {result.TotalAmount:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel: {ex.Message}");
        }
    }
    else if (cmd == "3")
    {
        Console.Write("ReservationId: ");
        var rid = Console.ReadLine();
        try
        {
            var order = orderService.Confirm(evt, rid!);
            Console.WriteLine($"Köp bekräftat. OrderId: {order.OrderId}. Summa: {order.TotalAmount:C}. Platser: {string.Join(",", order.SeatIds)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel: {ex.Message}");
        }
    }
    else if (cmd == "4")
    {
        foreach (var r in evt.ActiveReservations())
        {
            Console.WriteLine($"{r.ReservationId} | {r.BuyerEmail} | Seats: {string.Join(",", r.SeatIds)} | Expires: {r.ExpiresAt:HH:mm:ss}");
        }
    }
    else if (cmd == "5")
    {
        Console.WriteLine("Simulerar att 11 minuter passerar (reservationer löper ut)...");
        System.Threading.Thread.Sleep(300);
        evt.AdvanceClock(TimeSpan.FromMinutes(11));
        Console.WriteLine("Klart.");
    }
    else if (cmd == "6") break;
}
static bool IsValidEmail(string? email)
{
    if (string.IsNullOrWhiteSpace(email))
        return false;

    var atIndex = email.IndexOf('@');
    var dotIndex = email.LastIndexOf('.');

    return atIndex > 0 &&
           dotIndex > atIndex + 1 &&
           dotIndex < email.Length - 1;
}

