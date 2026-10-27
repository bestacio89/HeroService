# Kafka deployment readiness

This document describes the operational configuration required to run the HeroService Consumer with Kafka. It does not define a new cross-service event contract.

## Runtime wiring

The Consumer host registers the Franz.Common Kafka producer/consumer infrastructure and the hosted listener. The listener subscribes only to the explicitly configured `Messaging:Kafka:TopicName`; it must not infer topics from the Consumer assembly name.

Kafka is configured as an opt-in deployment concern: `appsettings.json` deliberately leaves the bootstrap server and topic empty. Startup fails until deployment configuration supplies them. The Development profile overrides these values for a local plaintext broker.

## Production environment variables

Supply the following through the deployment environment or a secret manager. Do not commit broker credentials.

| Environment variable | Purpose |
|---|---|
| `Messaging__Kafka__BootstrapServers` | Comma-separated broker endpoints |
| `Messaging__Kafka__GroupId` | Stable consumer group for this deployment |
| `Messaging__Kafka__TopicName` | Explicit HeroService-owned input/output topic |
| `Messaging__Kafka__Security__SecurityProtocol` | `SaslSsl` or `Ssl` |
| `Messaging__Kafka__Security__SaslMechanism` | SASL mechanism when using `SaslSsl` |
| `Messaging__Kafka__Security__SaslUsername` | Secret username for SASL |
| `Messaging__Kafka__Security__SaslPassword` | Secret password for SASL |
| `Messaging__Kafka__Security__SslCaLocation` | Optional CA certificate path |
| `Messaging__Kafka__Failure__DeadLetterTopic` | Optional explicit dead-letter topic; defaults to the input topic plus `-dlt` |
| `Messaging__Kafka__Failure__MaxRetryAttempts` | Bounded retries after the first processing attempt |
| `Messaging__Kafka__Failure__RetryBackoffMs` | Delay between retry attempts |

The Docker launch profile uses the `Docker` environment and loads `appsettings.docker.json`. Supply the broker credentials and any environment-specific endpoint overrides through the container environment.

## Delivery behavior

- Consumer auto-commit and auto-offset-store remain disabled.
- A record is committed only after the handler succeeds, or after the broker confirms that the failed raw record was published to the dead-letter topic.
- Handler failures receive a bounded retry policy. Deserialization failures are dead-lettered without retry because retrying the same malformed bytes cannot repair them.
- The dead-letter record preserves the original payload and key (by default) and adds source topic, partition, offset, failure type, failure message, and failure timestamp headers.
- If dead-letter publication fails, the original offset is not committed. The consumer must not silently discard the record.
- Processing is at-least-once, not exactly-once. Business handlers must be idempotent or use an idempotency key at their own persistence boundary.

## Ownership and release gate

This wiring is infrastructure only. HeroService must not consume MatchService lifecycle/results/snapshots or UserService account/progression data. Before production traffic is enabled, the team must define the concrete HeroService-owned message contract and register its handler; an unknown event must not be treated as successfully applied.

The Franz.Common change that implements retry/dead-letter/offset semantics must be merged, released to NuGet.org, and consumed at that released version before this service is considered production-ready. CI must validate the exact package graph using NuGet.org-only restore.
