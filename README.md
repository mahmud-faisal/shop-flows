# ShopFlows

An e-commerce order and fulfillment platform built to learn every GoF design pattern, plus enterprise patterns, in .NET 10. Pattern clarity matters more than feature volume.

## Layout

```
src/
  ShopFlows.Domain          entities, value objects, business rules (no dependencies)
  ShopFlows.Application     use cases (references Domain)
  ShopFlows.Infrastructure  persistence and fakes for external services (references Application, Domain)
  ShopFlows.Api             ASP.NET Core Web API (references all)
tests/
  ShopFlows.UnitTests        xUnit + FluentAssertions
  ShopFlows.IntegrationTests WebApplicationFactory-based API tests
PatternKatas/               standalone console demos, one folder per pattern
docs/PATTERN_MAP.md         where each pattern lives and why
magic-behind/               one explanation file per pattern, per day
```

Inside each layer, code is organized by feature folder (Catalog, Orders, Identity, Common, ...).

## Prerequisites

- .NET SDK 10.0

## Commands

```bash
# Build everything
dotnet build ShopFlows.slnx

# Run all tests
dotnet test ShopFlows.slnx

# Run only unit tests
dotnet test tests/ShopFlows.UnitTests

# Run the API
dotnet run --project src/ShopFlows.Api
#   OpenAPI document: http://localhost:5105/openapi/v1.json
#   Swagger UI:       http://localhost:5105/swagger
#   Scalar:           http://localhost:5105/scalar

# List or run pattern katas
dotnet run --project PatternKatas
dotnet run --project PatternKatas -- <PatternName>
```

Warnings are treated as errors (see `Directory.Build.props`).
