# ConfigSync

ASP.NET Core (Minimal API) service for submitting command execution plans to network devices via SSH.

**Stack:** ASP.NET Core · Dapper + Npgsql (no EF) · PostgreSQL · Orleans · Docker

---

## Architecture

Hexagonal (Ports & Adapters) with four layers:

| Layer | Role |
|---|---|
| `ConfigSync.Domain` | Pure data — `Device`, `ExecutionPlan`, `ExecutionReference` |
| `ConfigSync.Application` | Use cases + ports — `IExecuteCommandUseCase`, `ExecuteCommandService` |
| `ConfigSync.Adapters` | REST in (`In/Rest/`) · Postgres persistence out (`Out/Persistence/Postgres/`) |
| `ConfigSync` | Composition root — wires Serilog, OpenTelemetry, DI, Swagger |

---

## Local environment

Start Postgres + mock devices:
```bash
./local-dev/local_dev_start.sh
./local-dev/local_dev_stop.sh
```

Build:
```bash
dotnet build
```

test:
```bash
dotnet test --filter "Category!=Integration"
```

integration-test:
```bash
dotnet test --filter "Category=Integration"
```

List packages:
```bash
dotnet list package
```

DB schema (Atlas — manual approval required):
```bash
./local-dev/db_plan.sh    # preview
./local-dev/db_apply.sh   # apply
./local-dev/db_clean.sh   # wipe (destructive, on-demand only)
```

---

## API

Interactive docs available at http://localhost:5000/swagger when running locally.

| Method | Path | Description |
|---|---|---|
| `POST` | `/v1/executions` | Submit commands to run against a set of devices |

```bash
curl -X POST http://localhost:5000/v1/executions \
  -H "Content-Type: application/json" \
  -d '{
    "commands": ["show version", "show interfaces"],
    "deviceIds": ["device1", "device2"]
  }'
```

---

## Mock device simulator

`local-dev/mock-devices/` contains fake network devices for manually testing SSH-based device interaction. Each device is a real `sshd` server (via `panubo/docker-sshd`) with a `ForceCommand` that intercepts commands and returns fixed, realistic-looking output.

### Connecting to a device

```bash
ssh -i local-dev/mock-devices/keys/mock_devices_key -p 2201 device@localhost "show version"
```

| Device | Port |
|---|---|
| device1 | 2201 |

The SSH key pair (`local-dev/mock-devices/keys/`) only unlocks these local mock devices. To regenerate:

```bash
ssh-keygen -t ed25519 -f local-dev/mock-devices/keys/mock_devices_key -N "" -C "mock-devices-local-testing"
```

### Adding a new device

1. Create `local-dev/mock-devices/deviceN/respond.sh` — a `case "$SSH_ORIGINAL_COMMAND"` script returning canned text per command.
2. Create `local-dev/mock-devices/deviceN/force-command.conf` — one line: `ForceCommand /etc/mock/respond.sh`.
3. Add a `deviceN` service block to `local-dev/docker-compose.yml`, mounting both files plus the shared public key, on a new host port.