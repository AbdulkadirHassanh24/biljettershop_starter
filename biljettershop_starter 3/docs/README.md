# Biljettshoppen — Iteration 1 (MVP)

**Mål Iteration 1:**
- Event & Venue med platser (stolar/bänkar; lyxloge reserverad för framtiden).
- Reservationer med **10-minuters timeout** och köp.
- **Max 5 biljetter** per köpare (undantag: **familjeevent**).
- Enkel **prissättning** via Strategy.
- In-memory (ingen persistens ännu).

## Kör
```bash
dotnet build
dotnet run --project src/TicketShop
```
