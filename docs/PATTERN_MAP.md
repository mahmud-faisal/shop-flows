# Pattern Map

Where each pattern lives in ShopFlows, the problem it solves there, and when not to reach for it.

| Day | Pattern | File | Problem solved | When NOT to use |
|-----|---------|------|----------------|-----------------|
| 01 | Value Object | `src/ShopFlows.Domain/Common/Money.cs` | Prices and totals as bare `decimal` allowed negative amounts, missing currency, and USD + EUR arithmetic. `Money` validates on creation, is immutable, compares by value, and refuses cross-currency math. | When the concept has its own identity and lifecycle (Customer, Order), or is a plain number with no rules (page size). |
| 01 | Aggregate | `src/ShopFlows.Domain/Orders/Order.cs` | Order lines could be edited from anywhere, breaking rules such as draft-only edits, one currency per order, active products only, and max 99 units per line. `Order` is the single entry point for changing its `OrderItem`s. | For lookup data with no cross-object invariants (Category), or to pull in related but independent entities (Customer, Product). Reference those by Id instead. |
