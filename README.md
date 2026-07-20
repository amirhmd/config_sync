# ConfigSync

A resilient configuration ingestion and serving API.

Periodically fetches configuration data from public sources, upserts it into PostgreSQL with no duplicates, and serves it via a documented REST API with full observability.

## How it works

1. A scheduled background process fetches data from external sources.
2. Records are normalized into a canonical shape.
3. Each record is upserted into PostgreSQL (idempotent — safe to re-run).
4. A REST API (Swagger/OpenAPI) exposes the synced data.
5. Traces, metrics, and logs are emitted via OpenTelemetry.

## Stack

ASP.NET Core (Minimal API, Kestrel) · Dapper + Npgsql (no EF) · PostgreSQL · Orleans · JWT auth · OpenTelemetry · Docker

## Architecture

Hexagonal: `Domain` (data only) → `Application` (use cases + ports) → `Adapters` (REST, persistence) / `Infrastructure` (connection/client setup) → `ConfigSync` (composition root, host).

## Requirements

- Idempotent ingestion, re-running never duplicates or corrupts data
- REST API with Swagger, protected write endpoints (JWT + policies)
- End-to-end traceability of ingestion runs and API requests
- Retry/backoff on transient external or DB failures
- Externally configurable sources, schedules, connections

## Running locally

Start:
```bash
./local-dev/local_dev_start.sh
```

Stop:
```bash
./local-dev/local_dev_stop.sh
```