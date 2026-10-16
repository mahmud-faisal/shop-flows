# ShopFlows

E-commerce order and fulfillment platform built to learn every GoF design pattern plus enterprise patterns in .NET. Code quality and pattern clarity matter more than feature volume.

## Stack
- .NET 10, ASP.NET Core Web API, EF Core with SQL server, xUnit, FluentAssertions, Moq or NSubstitute
- MediatR is introduced only on Day 30. Before that, use our own hand-written mediator (Day 24).
- Polly for resilience (Day 33).

## Solution layout
src/ShopFlows.Domain, src/ShopFlows.Application, src/ShopFlows.Infrastructure, src/ShopFlows.Api
tests/ShopFlows.UnitTests, tests/ShopFlows.IntegrationTests
PatternKatas/ (console project, one folder per pattern)
docs/PATTERN_MAP.md
magic-behind/ (one folder per day, one explanation file per pattern inside it, see Magic-behind rules)

Dependency rule: Domain references nothing. Application references Domain. Infrastructure references Application and Domain. Api references all.
Inside each layer, organize by feature folder: Catalog, Pricing, Cart, Orders, Inventory, Shipping, Payments, Notifications, Returns, Reporting, Identity, Common.

## Pattern rules
- Every pattern implementation lives in its own clearly named folder or file.
- Put a header comment on the main file: Pattern name, Problem it solves here, Why this pattern, When NOT to use it.
- Each pattern gets a standalone kata in PatternKatas/<PatternName> (no frameworks, console demo with a Program entry) BEFORE it is applied in ShopFlows.
- Do not use a pattern that is not in the current day's scope. Do not over-engineer.
- No real external services. Payment gateways and couriers are fake in-process classes.

## Daily workflow (every day prompt means this)
1. Build the kata in PatternKatas first.
2. Apply the pattern in ShopFlows exactly as the day prompt describes.
3. Add unit tests for the pattern behavior. Add API-level tests when an endpoint is added.
4. Update docs/PATTERN_MAP.md with a row: pattern, file path, problem solved, when NOT to use.
5. Run dotnet build and dotnet test. Fix everything until green.
6. Only after everything is green, write the magic-behind explanation file(s) for today's pattern(s) and update magic-behind/README.md (rules below).
7. Commit with message: Day NN: <pattern> - <feature>. Include the magic-behind file in the commit. Do not tag; I tag manually.
8. End by printing a short summary: files added, patterns used, magic-behind file path, anything left for later.

## Magic-behind rules (explanation file after every day's task)
Put each day's files in their own folder: magic-behind/Day_<NN>-<Topic-Name>/, at the repo root. NN is the two-digit day number and Topic-Name is the day's topic in Title-Case words joined by hyphens, for example magic-behind/Day_04-Factory-Method/.
Inside that folder, name each file <NN>-<pattern-name>.md in kebab-case, for example magic-behind/Day_04-Factory-Method/04-factory-method.md.
- If a day covers several patterns, create one file per pattern in that day's folder.
- Review, checkpoint, and integration days create a file only if a pattern was changed in a meaningful way.

Every file must contain exactly these five sections, in this order:

## 1. Why this pattern was needed here
The concrete problem in ShopFlows before the pattern. Show the naive or bad code shape (if/else chains, tight coupling, duplication) and the pain it caused.

## 2. How the pattern works (core concept)
The idea in plain language: participants and their roles, a Mermaid class or sequence diagram, and a minimal generic C# example unrelated to ShopFlows.

## 3. How it is implemented in this project
Exact file paths and class names. Walk through the real code with short excerpts, show how it is registered in DI, name the endpoint or test that exercises it, link to the kata folder, and say which other features it connects to.

## 4. Pros and cons
Two lists. Make them specific to this codebase, not generic textbook points.

## 5. When to use and when to avoid
Use it when..., avoid it when..., and the closest alternative pattern with one sentence on how to choose between them.

Writing rules:
- Write for a learner who knows C# but not this pattern.
- Keep each file between 500 and 900 words.
- Code excerpts must be copied from the real files, never invented. Re-read the code before writing to confirm the file matches what exists.
- Maintain magic-behind/README.md as an index table: day, pattern, link to the file (path includes the day folder).

## Conventions
- Async everywhere with CancellationToken. Nullable reference types on. Primary constructors allowed.
- Money is a value object (decimal amount + currency). No primitive obsession.
- Entities hold behavior. No anemic domain model.
- Controllers are thin: map request, call application layer, return result.
- Public APIs return ProblemDetails for errors once Day 31 is done.
- Ask me before adding any NuGet package not listed in the stack.
- Don't commit anything
