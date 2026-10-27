# HeroService quality and Blazor modernization plan

Target branch: `Develop_Unity_V1`

This plan is an implementation checklist, not a claim that the work is already complete. Tests must describe the behavior implemented by the current code and its production wiring; they must not manufacture passing outcomes by weakening assertions or replacing real infrastructure with an in-memory substitute.

## Definition of done

- [x] The solution restores using NuGet.org only and builds in Release on the supported .NET SDK.
- [ ] Every non-Unity production project is mapped to unit, integration, contract, architecture, or browser-level tests as appropriate.
- [ ] Existing test projects are inventoried, repaired or migrated, and included in the intended solution/CI execution path; obsolete projects are not silently ignored.
- [x] The initial unit tests are deterministic, isolated and fast; pure domain/application logic does not launch containers.
- [x] Initial integration tests use Testcontainers where real infrastructure matters. PostgreSQL tests run against PostgreSQL, and API tests use PostgreSQL plus Redis rather than in-memory substitutes.
- [x] Current Testcontainers images are pinned; container lifecycle and per-test data isolation are explicit.
- [ ] API integration tests exercise the real HTTP pipeline, dependency injection, Franz.Common registration, persistence and error handling.
- [ ] Client tests verify HTTP methods, paths, query parameters, serialization, status handling, cancellation, and malformed responses.
- [ ] Consumer tests exercise actual message-processing semantics. Acknowledgement, retry, idempotency and dead-letter behavior are asserted only where the configured implementation provides them.
- [ ] Architecture tests enforce dependency direction, project boundaries, and Franz.Common lifetime/registration conventions without duplicating rule definitions across test and report projects.
- [ ] Blazor pages have explicit loading, error, empty, success and in-progress states; interactions are keyboard-accessible and responsive.
- [ ] Critical browser journeys have end-to-end coverage, and UI performance claims are backed by measurements rather than assumptions.
- [x] CI runs unit/architecture and container-backed suites separately and publishes TRX results. Container-backed tests are not silently skipped when Docker or required services are unavailable.
- [ ] The pull request contains a clear summary, implementation details, test evidence, known limitations and release notes. It is squash-merged only after required checks pass.

## Current solution inventory

The current `HeroService.slnx` includes these non-Unity production projects:

| Project | Primary test responsibility |
|---|---|
| `HeroService.Domain` | Entity invariants, value objects, domain rules and boundary cases |
| `HeroService.Contracts` | DTO/command/query shape, validation and serialization contracts |
| `HeroService.Application` | Handler behavior, orchestration, mappings, cancellation and failure paths |
| `HeroService.Persistence` | Repository query behavior and PostgreSQL-specific persistence integration |
| `HeroService.API` | HTTP routing, status codes, request/response contracts, middleware and production DI |
| `HeroService.Client.Http` | HTTP contract tests using a controlled message handler, plus API contract tests |
| `HeroService.Consumer` | Message processing and configured broker/database integration |
| `HeroService.Admin` | bUnit component tests and Playwright critical journeys |

Existing test projects in the solution:

- `TestProject1/HeroService.Testing.csproj`: currently references API, Application, Contracts, Domain and Persistence; it already contains architecture tests.
- `tests/HeroService.Testing.ArchitecturalReports/HeroService.Testing.ArchitecturalReports.csproj`: references the main testing project and produces reports. Shared architectural invariants should have one authoritative rule definition.
- `tests/HeroService.Testing.ClientHttp`: newly added focused tests for typed HTTP client request contracts.
- `tests/HeroService.Testing.Domain`: tests for GameVersion lifecycle and stat/modifier boundary invariants.
- `tests/HeroService.Testing.Application`: tests for SnapshotResolver active-version and modifier-version preconditions.
- `tests/HeroService.Testing.Persistence.Integration`: PostgreSQL Testcontainers schema/persistence smoke test.
- `tests/HeroService.Testing.API.Integration`: real API host tests with PostgreSQL and Redis Testcontainers.

A repository-level `HeroService.Consumer.Tests` project exists outside the solution. Its current source references legacy `HeroService.Common.*` namespaces/package names, uses a floating Kafka image, hardcodes the broker address despite random port binding, and skips its only integration test. It must be migrated to the current Franz.Common-based consumer and made executable before it is considered part of the coverage baseline.

## Execution plan

### Phase 1 — Baseline and delivery gates

- [x] Record production/test project inventory and existing test gaps.
- [x] Replace the stale .NET 6 / traditional-solution GitHub workflow with .NET 10, `HeroService.slnx`, NuGet.org-only restore and test artifact publication.
- [x] Capture a clean baseline build and test result in CI; resolve failures without weakening existing architecture rules.

### Phase 2 — Domain, contracts and application

- [x] Add initial tests against GameVersion lifecycle and HeroBaseStats/HeroModifier boundary invariants.
- [ ] Expand domain coverage to the remaining entities, value objects and business rules.
- [x] Add initial SnapshotResolver tests for active-version and modifier-version preconditions.
- [ ] Cover remaining application handlers with focused fakes at boundaries, not mocks of every internal call.
- [ ] Verify request/response DTO serialization and validation contracts.
- Preserve deterministic behavior and cancellation/error propagation.

### Phase 3 — Persistence and API

- [ ] Consolidate the current pinned PostgreSQL Testcontainers setup into a reusable fixture with readiness checks.
- [x] Apply real EF migrations to a clean PostgreSQL database in API integration tests.
- [ ] Verify schema constraints, repository filters, graph loading, transactions and seed behavior.
- [x] Use the real API host and production service registrations for initial HTTP integration tests; expand endpoint and error-path coverage.
- Never replace PostgreSQL with SQLite or EF Core InMemory for persistence correctness tests.

### Phase 4 — Client and messaging

- Test typed HTTP clients with deterministic fake handlers, including failure and cancellation paths.
- Migrate the stale consumer test project to current contracts and Franz.Common registrations.
- Add a pinned Kafka Testcontainers fixture only after the broker configuration and acknowledgement/retry semantics are understood; assert only behavior the implementation guarantees.
- Add database/broker integration tests for cross-resource workflows only where such workflows actually exist.

### Phase 5 — Architecture and Blazor quality

- Consolidate architecture invariants and remove report/test rule duplication.
- Add bUnit tests for loading, empty, error, form validation, dialog and notification behavior.
- Add Playwright coverage for core admin journeys.
- Measure rendering and request behavior before introducing virtualization, caching or state-management abstractions.

### Phase 6 — CI and release readiness

- [x] Separate fast unit/architecture tests from container-backed integration tests.
- [x] Run container-backed suites on a Docker-capable runner and publish TRX artifacts.
- [ ] Add coverage collection and enforce agreed coverage thresholds.
- Validate Bicep and container configuration in their own gates; do not make cloud resources a prerequisite for ordinary unit tests.
- Require all checks to pass before the PR is ready for squash merge.

## Known findings to resolve, not conceal

- Resolved during this implementation: the active GitHub Actions workflow now targets .NET 10 and `HeroService.slnx`, restores from NuGet.org only, separates unit and integration suites, and publishes TRX results.
- The consumer's current startup calls a SQL Server database extension while the API uses the relational database registration path; configuration parity must be investigated before claiming consumer/database integration is correct.
- `GenericMessageProcessor<T>` currently deserializes and logs but its entity-processing method contains no persistence or business action. Tests must expose this current behavior; they must not imply that a message has been durably processed.
- The existing consumer test project is not included in the solution and contains stale references and a skipped integration test.
- Resolved during this implementation: two modifier controllers violated the established namespace rule; the Domain audit expected `HeroService.Common` rather than `Franz.Common`; `HeroAffiliation` and `GameVersion` had no namespace; and the API cache key prefix `heroservice:` violated its own alphanumeric validation. Production code/configuration and the current EF model snapshot were corrected without relaxing architecture rules.
- CI restore still reports high-severity advisories for transitive `System.Security.Cryptography.Pkcs` and `System.Security.Cryptography.Xml` packages. The patched 10.0.13 release is documented in [GHSA-846f-f764-76mx](https://github.com/advisories/GHSA-846f-f764-76mx) and [GHSA-7hqp-7c5m-4vq6); package remediation remains open and must be completed before release.
- The UI is an Interactive Server Blazor app. Browser tests and performance measurements must account for its server connection and request latency.
