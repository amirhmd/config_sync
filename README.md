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

db verifications
```bash
docker exec -it configsync-postgres psql -U configsync -d configsync -c '\d+ public.devices'
```

Build:
```bash
dotnet build
```

> **Note:**
> If you see the error below, Docker is not running or is not accessible through `/var/run/docker.sock`.
>
> Start Docker before running the tests, as the integration tests require it to verify the PostgreSQL health check and database connectivity.
>
> ```text
> System.AggregateException: One or more errors occurred.
> Failed to connect to Docker endpoint at 'unix:///var/run/docker.sock'.
> System.Net.Http.HttpRequestException: Connection failed.
> ```


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

## Manual API testing

Start the service:

```bash
dotnet run --project src/ConfigSync
```

Health:

```bash
curl -s -o /dev/null -w "live=%{http_code}\n" http://localhost:5000/health/live
curl -s -o /dev/null -w "ready=%{http_code}\n" http://localhost:5000/health/ready
```

Create with password:

```bash
curl -s -X POST http://localhost:5000/v1/devices -H "Content-Type: application/json" \
  -d '{"name":"router_01","host":"localhost","port":2201,"username":"device","password":"secret123"}' | jq
```

Create with private key:

```bash
curl -s -X POST http://localhost:5000/v1/devices -H "Content-Type: application/json" \
  -d '{"name":"switch_core","host":"10.0.0.1","port":22,"username":"admin","privateKey":"-----BEGIN OPENSSH PRIVATE KEY-----"}' | jq
```

Get:

```bash
curl -s http://localhost:5000/v1/devices/router_01 | jq
```

List:

```bash
curl -s "http://localhost:5000/v1/devices?limit=10" | jq
```

Delete:

```bash
curl -i -X DELETE http://localhost:5000/v1/devices/router_01
```

Create a `psql` helper for the local database:

```bash
psql() {
  docker exec -i configsync-postgres \
    psql -U configsync -d configsync "$@"
}
```

Then query it normally:

```bash
psql -c "SELECT * FROM devices;"
```

Inspect credentials without printing ciphertext:

```bash
psql -c "
SELECT
    name,
    password_encrypted IS NOT NULL AS has_password,
    private_key_encrypted IS NOT NULL AS has_private_key
FROM devices;"
```

Inspect the schema:

```bash
psql -c "\d devices"
```
