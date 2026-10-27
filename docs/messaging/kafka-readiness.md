# Kafka consumer deployment readiness

This document records the HeroService-owned readiness work for deploying the Consumer with Kafka. It is not a claim that a Kafka broker is provisioned or that the current Consumer performs domain-side message handling.

## Current architecture boundary

HeroService may consume messages only when they cause behavior owned by the Hero domain (for example, applying a hero-owned configuration event). Match lifecycle, match outcomes, gameplay snapshots, game-version selection, and player progression remain owned by their respective services. A consumer must not create cross-service database writes or duplicate those domains.

The current Consumer initializes Franz.Common messaging and persistence, but its generic processor only deserializes/logs payloads. No business side effect is configured. Kafka deployment is therefore an infrastructure option until HeroService-owned event contracts and handlers are explicitly defined.

## Required delivery contract

Before enabling a real subscription in a deployed environment:

- Use manual offset commit and offset store; do not enable Kafka auto-commit.
- Process one record at a time per partition unless ordering and concurrency semantics are deliberately designed and tested.
- Commit only after the handler's durable side effect succeeds.
- Treat delivery as at-least-once. Handlers must be idempotent; Kafka offset commit and a database transaction are not a distributed atomic transaction.
- Do not acknowledge handler exceptions, invalid envelopes, or unsupported message types as successful.
- Define bounded retry and a dead-letter/recovery strategy for poison messages. A partition must not enter an unbounded restart/replay loop without operational visibility.
- Preserve message identity, correlation metadata, topic, partition, and offset in logs/telemetry without logging sensitive payloads.
- Provide graceful shutdown so the in-flight handler finishes or is cancelled before the process exits.

## Configuration for deployment

The existing appsettings defaults are local-development values. Deployment configuration must override them through environment-specific configuration or the orchestrator's secret/configuration provider:

- `Messaging__Kafka__BootstrapServers`
- `Messaging__Kafka__GroupId`
- `Messaging__Kafka__Security__SecurityProtocol`
- SASL credentials or TLS certificate locations when the cluster requires them
- Consumer offset, session, and poll interval settings appropriate to the handler's maximum processing duration
- `Database__*` and `Caching__ConnectionString` only if a Hero-owned handler actually needs those dependencies

Do not commit production credentials. Use Kafka ACLs scoped to the Consumer's topic and group. Production must not use `Plaintext` when the broker requires authenticated/encrypted transport.

## Validation gates

- [ ] Franz.Common Kafka listener acknowledges only successful processing; handler exceptions propagate.
- [ ] Framework regression tests prove failed records remain uncommitted and are redelivered.
- [ ] Poison-message behavior is bounded, documented, and observable (retry/DLT or explicit operator recovery).
- [ ] HeroService defines only Hero-owned message contracts and handler side effects.
- [ ] Kafka Testcontainers integration tests validate actual topic consumption, dispatch, commit-after-success, redelivery-after-failure, and shutdown.
- [ ] Security advisories are resolved in the restored dependency graph.
- [ ] Consumer Docker image builds from the repository context with NuGet.org-only restore and runs as non-root.
- [ ] Deployment manifests supply broker endpoints, security, group ID, resource limits, probes/health semantics, and restart policy.
- [ ] CI and release evidence are green before the Consumer is enabled in a production deployment.

## Framework dependency

The current published Franz.Common 2.3.0 listener was found to commit after handler failure, while the hosted service catches and suppresses handler exceptions. The framework correction is tracked in [Franz.Common PR #151](https://github.com/bestacio89/Franz.Common/pull/151). HeroService must consume a released package containing that correction before Kafka can be considered safe to deploy. The PR is not yet merged/released, and this document intentionally does not assert the behavior is already available in the current package.
