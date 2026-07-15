<p align="center">
  <img width="200" src="./Docs/assets/FranzTemplate.png" alt="Franz Logo"/>
</p>

<h1 align="center">HeroService</h1>
<p align="center"><b>Versioned hero, skill, and balance-data service for a mythology-themed action game — built on Franz</b></p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10-blueviolet" />
  <img src="https://img.shields.io/badge/Architecture-Clean%20%7C%20DDD%20%7C%20CQRS-brightgreen" />
  <img src="https://img.shields.io/badge/Cache-Redis-red" />
  <img src="https://img.shields.io/badge/Database-PostgreSQL-336791" />
  <img src="https://img.shields.io/badge/Messaging-Kafka-orange" />
  <img src="https://img.shields.io/badge/MultiCloud-Azure%20%7C%20AWS%20%7C%20GCP-9cf" />
  <img src="https://img.shields.io/badge/IaC-Terraform%20%7C%20Bicep-success" />
  <img src="https://img.shields.io/badge/CI%2FCD-Azure%20DevOps%20%7C%20GitHub%20%7C%20GitLab-informational" />
</p>

---

## What this actually is

HeroService owns every hero in a mythology-themed action game — 18 heroes spanning Norse, Greek,
Egyptian, Japanese, Mesopotamian, and Hindu mythology, across six gameplay classes (Warrior,
Assassin, Mage, Tank, Support, Ranger). For each hero it owns: base stats, a five-slot skill kit
(Passive / Primary / Secondary / Tertiary / Ultimate), cosmetic skins, and — the part that actually
makes this interesting architecturally — **balance data that is versioned and immutable**, not
just mutable rows that get patched in place.

It's also the project I use to stress-test [Franz.Common](https://github.com/bestacio89/Franz.Common),
the framework underneath it — 61+ packages, 730k+ downloads on NuGet, sole-authored. HeroService is
where framework decisions get proven against a real, non-trivial domain before they ship.

## The core design decision: balance data is versioned, not mutated

A `GameVersion` is a frozen balance snapshot. `HeroModifier` and `SkillModifier` rows are scoped to
a specific `GameVersionId`, and once a version is published, its modifiers don't change — a
rebalance patch means creating a *new* `GameVersion`, not editing an old one in place. That one
decision is what makes everything downstream simple:

- **Deterministic snapshots.** `GetHeroSnapshotQuery(HeroId, GameVersionId)` and
  `BrowseHeroSnapshotsQuery(GameVersionId)` resolve a hero's fully-computed combat stats (base +
  modifiers + skill kit) for a given balance version. Same inputs, same output, always — no hidden
  time-dependency.
- **Caching without the usual invalidation headache.** Because a snapshot is pure given
  `(HeroId, GameVersionId)`, it can be cached aggressively (Redis, via `Franz.Common.Caching`)
  without a cache-invalidation strategy chasing every write — the only way a cached snapshot goes
  stale is a deliberate hotfix to an already-published version, which is a rare, ops-controlled
  event rather than a routine one.
- **Match Service reads once, not repeatedly.** The wider game architecture freezes an immutable
  per-match participant snapshot at match creation — no further queries to Hero or User aggregates
  once a match starts. HeroService's snapshot endpoints are exactly what makes that possible.

## Architecture

- **CQRS via `Franz.Common.Mediator`** — every read is a query handler, every write a command
  handler, no service-layer god classes.
- **Clean layering**: `HeroService.Domain` (entities, no framework dependencies) →
  `HeroService.Contracts` (DTOs, queries, commands, repository interfaces) →
  `HeroService.Application` (handlers) → `HeroService.Persistence` (EF Core, repositories, seeders)
  → `HeroService.API` (controllers) / `HeroService.Consumer` (Kafka event handling) /
  `HeroService.ClientHttp` (typed HTTP client for other services).
- **Architecture-as-code**: layer-dependency rules (no domain → infrastructure leakage, handler
  naming conventions, repository lifetime rules) are enforced by dedicated test projects, not just
  documented and hoped for.
- **EF Core + PostgreSQL**, with a full seeding pipeline: mythologies, origin archetypes, cultures,
  hero classes, all 18 heroes, all 90 skills (5 per hero), and their balance modifiers — the
  database comes up populated, not empty.

## Getting started

```bash
git clone https://github.com/bestacio89/HeroService.git
cd HeroService
docker-compose -f sources/back/HeroService.DockerCompose/docker-compose.yml up --build
```

That's Postgres, Redis, Kafka/Zookeeper, the API, and the Consumer — one command, seeded database,
no manual setup. API comes up on `http://localhost:8080`, Swagger at `/swagger`.

To run just the API against local tooling instead:

```bash
dotnet restore
dotnet run --project sources/back/HeroService.API
```

## API surface

| Controller | Owns |
|---|---|
| `HeroController` | Hero CRUD and lookups |
| `SkillController` | Skill definitions, effects, base stats |
| `SnapshotController` | Resolved, version-scoped hero snapshots (the cached, high-traffic endpoint) |
| `VersionController` | Game balance versions |
| `HeroProgressionController` | Per-level stat scaling |
| `SkillScalingModifierController` | Per-version skill balance modifiers |
| `HeroClassController` / `MythologyTypeController` / `OriginArchetypeController` / `OriginCultureController` | Identity/classification axes (class, mythology, archetype, culture) |

## Infrastructure as code

Real, non-toy IaC for all three major clouds, not just one demonstrated and the others implied:

```
Infrastructure/
├── AzureDevOps-Bicep/     # Azure: AKS, networking, KeyVault
├── Terraform-AWS/         # AWS: EKS/ECS, RDS, MSK/Amazon MQ, networking
└── Terraform-GCP/         # GCP: GKE/Cloud Run, Pub/Sub-equivalent, networking
```

Each cloud module toggles between Kafka and RabbitMQ, and between container-orchestrated
(EKS/GKE) and serverless (ECS/Cloud Run) compute, via Terraform variables — not three
copy-pasted, hard-coded stacks.

CI/CD is mirrored across three platforms with real per-cloud jobs, not one pipeline pretending to
support all three:

| Platform | Path |
|---|---|
| Azure DevOps | `pipelines/` |
| GitHub Actions | `.github/workflows/` |
| GitLab CI | `.gitlab/ci/` |

## Tech stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 |
| Framework | [Franz.Common](https://github.com/bestacio89/Franz.Common) (CQRS/Mediator, Caching, EF Core integration, Resilience, Hosting) |
| Database | PostgreSQL via EF Core |
| Cache | Redis (`Franz.Common.Caching`) |
| Messaging | Kafka (`HeroService.Consumer`) |
| Resilience | Polly (retries, circuit breaker, timeout) |
| Containerization | Docker, multi-stage builds, non-root runtime |

## Project structure

```
sources/back/
├── HeroService.Domain/         # Entities, value objects — no framework dependencies
├── HeroService.Contracts/      # DTOs, queries, commands, repository interfaces
├── HeroService.Application/    # Command/query handlers
├── HeroService.Persistence/    # EF Core, repositories, seeders
├── HeroService.API/            # Controllers, composition root
├── HeroService.Consumer/       # Kafka event consumption
└── HeroService.ClientHttp/     # Typed HTTP client for other services
```

## License

See [`LICENSE.txt`](./LICENSE.txt).