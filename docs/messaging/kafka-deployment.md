# Kafka deployment readiness

This document describes the operational configuration required to run the HeroService Consumer with Kafka. It does not define a new cross-service event contract.

## Runtime wiring

The Consumer host registers Franz.Common Kafka infrastructure and a hosted listener. The listener subscribes only to the explicit input-topic list in `Messaging:Kafka:Consumer:Topics`. The producer's `Messaging:Kafka:TopicName` is the outbound topic; input and output topics must remain distinct to prevent accidental self-consumption loops.

Kafka is an opt-in deployment concern: `appsettings.json` deliberately leaves the bootstrap server, outbound topic and input-topic list empty. Startup fails until deployment configuration supplies them. The Development profile overrides these values for a local plaintext broker. The outbox remains disabled until a concrete HeroService-owned outbound event and transactional persistence path are implemented.

## Production environment variables

Supply the following through the deployment environment or a secret manager. Do not commit broker, database or cache credentials. Production startup rejects local database/cache defaults and requires a non-local database with TLS enabled.

| Environment variable | Purpose |
|---|---|
| `Database__ServerName` | Production PostgreSQL host |
| `Database__DatabaseName` | Database name |
| `Database__UserName` | Database login supplied by deployment configuration |
| `Database__Password` | Database password supplied through a secret manager |
| `Database__SslMode` | TLS mode; must not be `Disable` outside Development |
| `Caching__ConnectionString` | Non-local Redis connection string, with credentials/TLS as required by the provider |

| Environment variable | Purpose |
|---|---|
| `Messaging__Kafka__BootstrapServers` | Comma-separated broker endpoints |
| `Messaging__Kafka__GroupId` | Stable consumer group for this deployment |
| `Messaging__Kafka__TopicName` | Explicit HeroService outbound topic |
| `Messaging__Kafka__Consumer__Topics__0` | First HeroService-owned input topic; add indexed entries for additional topics |
| `Messaging__Kafka__Security__SecurityProtocol` | `SaslSsl` or `Ssl` |
| `Messaging__Kafka__Security__SaslMechanism` | SASL mechanism when using `SaslSsl` |
| `Messaging__Kafka__Security__SaslUsername` | Secret username for SASL |
| `Messaging__Kafka__Security__SaslPassword` | Secret password for SASL |
| `Messaging__Kafka__Security__SslCaLocation` | Optional CA certificate path |
| `Messaging__Kafka__Failure__DeadLetterTopic` | Optional explicit dead-letter topic; defaults to the input topic plus `-dlt` |
| `Messaging__Kafka__Failure__MaxRetryAttempts` | Bounded retries after the first processing attempt |
| `Messaging__Kafka__Failure__RetryBackoffMs` | Delay between retry attempts |

The Docker launch profile uses the `Docker` environment and loads `appsettings.docker.json`. Supply broker credentials and environment-specific endpoint overrides through the container environment.

## Broker-side prerequisites

- Pre-create the configured input, output and dead-letter topics through the platform provisioning process. Set replication factor, retention, partition count and minimum in-sync replicas to match the deployment SLA; do not rely on broker auto-topic creation.
- Grant the service principal only the required ACLs: read/describe on configured input topics and the consumer group, write/describe on the outbound topic, and write/describe on the dead-letter topic.
- Use the same consumer group ID across replicas of one deployment and a different group ID for each environment.
- Provision dead-letter topic retention and alerting before enabling production traffic. A successful DLT publish prevents the source record from blocking the partition, so DLT messages must be reviewed and replayed through a controlled process.

## Operational signals

Monitor consumer lag, repeated processing retries, DLT publish counts, consumer restarts, broker connection failures and database/cache failures. Alert on any DLT growth and define an operational owner for replay and remediation.

- `GET /health/live` reports process liveness without depending on external services, preventing broker outages from causing restart storms.
- `GET /health/ready` checks broker metadata and the configured input, output and dead-letter topics. It returns unhealthy until all required topics are visible to the service principal.
- The container listens on port `8080` by default. Override `ASPNETCORE_URLS` if the deployment platform requires a different binding.

## Delivery behavior

- Consumer auto-commit and auto-offset-store remain disabled.
- A record is committed only after the handler succeeds, or after the broker confirms that the failed raw record was published to the dead-letter topic.
- Handler failures receive bounded retries. Deserialization failures are dead-lettered without retry because retrying the same malformed bytes cannot repair them.
- The dead-letter record preserves the original payload and key by default and adds source topic, partition, offset, failure type, failure message and failure timestamp headers.
- If dead-letter publication fails, the original offset is not committed. The consumer must not silently discard the record.
- Processing is at-least-once, not exactly-once. Business handlers must be idempotent or use an idempotency key at their own persistence boundary.

## Ownership and release gate

This wiring is infrastructure only. HeroService must not consume MatchService lifecycle/results/snapshots or UserService account/progression data. Before production traffic is enabled, define the concrete HeroService-owned message contract and register its handler; unknown events must not be treated as successfully applied.

The Franz.Common change that implements retry/dead-letter/offset semantics must be merged, released to NuGet.org, and consumed at that released version before this service is considered production-ready. CI must validate the exact package graph using NuGet.org-only restore.
