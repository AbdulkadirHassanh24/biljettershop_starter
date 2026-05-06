# Stegplan

**Iter 1 (nu):**
- Domän: Event, Venue, Seat (Chair/Bench/LuxuryBox), Reservation, Order
- Policies: SeatLimitPolicy (max 5), ReservationExpiryPolicy (10 min)
- Pricing: IPriceStrategy + BasicPriceStrategy
- Services: ReservationService, OrderService
- CLI: Lista, Reservera, Bekräfta, Visa reservationer, Simulera timeout

**Iter 2:**
- Persistens (XML/JSON), betalning (faktura/direkt), bekräftelse-ID
- Prissättning per tid/typ, fil-I/O, konfig & logg

**Iter 3 (VG):**
- Samtidighet/låsning, zoner/prisnivåer, VIP-entré-logik
- UI/GUI eller API + frontend
