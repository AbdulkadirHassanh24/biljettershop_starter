using TicketShop.Domain;

namespace TicketShop.Tests;

public static class TestData
{
    public static Event CreateEvent(bool isFamilyEvent = false)
    {
        var venue = DemoData.CreateVenue();
        return DemoData.CreateEvent(venue, isFamilyEvent);
    }
}