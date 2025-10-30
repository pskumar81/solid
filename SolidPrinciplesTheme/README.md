# SOLID Principles Wildlife Tour Theme

This console project demonstrates every SOLID principle inside a single wildlife tour booking domain.

| Principle | Implementation | Notes |
|-----------|----------------|-------|
| **Single Responsibility** | `TourAvailabilityService`, `FamilyFriendlyPolicy` | Each class focuses on a single concern like availability or rule validation. |
| **Open/Closed** | `ITourPricingStrategy`, `StandardPricingStrategy`, `PeakSeasonPricingStrategy` | New pricing variants extend the strategy without touching consumers. |
| **Liskov Substitution** | `Tour` base type with `WalkingTour` and `BoatTour` | Derived tours honour the base guarantees (`CanAccommodateGroup`) so callers can substitute safely. |
| **Interface Segregation** | `ITourLookup`, `ITourScheduleReader`, `ITourBookingWriter` | Read/write responsibilities are split so consumers depend only on what they need. |
| **Dependency Inversion** | `TourBookingService` | High-level orchestration depends on abstractions (pricing, policies, notifications, repositories). |

Run the sample with:

```bash
cd SolidPrinciplesTheme
dotnet run --project SolidPrinciplesTheme.App
```

The console output shows a booking scenario with confirmation and notifications while exercising each principle through the tour booking flow.
