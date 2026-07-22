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

## Mock device simulator

`local-dev/mock-devices/` contains fake network devices, used for manually testing SSH-based device interaction.

Each device is a real `sshd` server (via `panubo/docker-sshd`) with a `ForceCommand` that intercepts whatever the client 
asks to run, matches it against a set of canned responses, and returns fixed, realistic-looking output instead of actually 
executing anything.


### Connecting to a device

```bash
ssh -i local-dev/mock-devices/keys/mock_devices_key -p 2201 device@localhost "show version"
```

| Device | Port |
|---|---|
| device1 | 2201 |

The SSH key pair (`local-dev/mock-devices/keys/`) only unlocks these local mock devices.

If you ever need to regenerate it:

```bash
ssh-keygen -t ed25519 -f local-dev/mock-devices/keys/mock_devices_key -N "" -C "mock-devices-local-testing"
```

### Adding a new device

1. Create `local-dev/mock-devices/deviceN/respond.sh` — a `case "$SSH_ORIGINAL_COMMAND"` script returning canned text per command.
2. Create `local-dev/mock-devices/deviceN/force-command.conf` — one line: `ForceCommand /etc/mock/respond.sh`.
3. Add a `deviceN` service block to `local-dev/docker-compose.yml`, mounting both files plus the shared public key, on a new host port.

## Running locally

Start:
```bash
./local-dev/local_dev_start.sh
```

Stop:
```bash
./local-dev/local_dev_stop.sh
```