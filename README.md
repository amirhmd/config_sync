# ConfigSync
An ASP.NET Core (Minimal API) service for submitting command execution plans to network devices (routers/switches). 
a client posts a list of commands + a list of device IDs, and the system coordinates running those commands against 
the target devices via SSH.

. ASP.NET Core (Minimal API) 
· Dapper + Npgsql (no EF) 
· PostgreSQL 
· Orleans 
· Docker.

## Architecture — Hexagonal (Ports & Adapters)

It follows strict hexagonal architecture, with 4 layers:ConfigSync.Domain pure data, no dependencies
* Device.cs a network device: Id, Host, Port, Username, Password?, PrivateKey?
* ExecutionPlan.cs immutable list of Commands + DeviceIds 
* ExecutionReference.cs a Guid returned to the caller as a tracking ID

ConfigSync.Application use cases + ports
* IExecuteCommandUseCase.cs the inbound port: Execute(ExecutionPlan) → ExecutionReference
* ExecuteCommandService.cs current implementation generates a Guid, logs, increments an OTel counter, and returns. 

ConfigSync.Adapters inbound REST + outbound persistence stubs
* ExecuteCommandEndpoint.cs maps HTTP request → domain → calls use case → returns 201 Created with a Location header and ReferenceId
* ExecutionPlanMapper.cs converts ExecuteCommandRequest (REST model) → ExecutionPlan (domain)
* DeviceEntity.cs DB row shape for a device
* RestEndpointMappings.cs registers POST /v1/executions

ConfigSync.Infrastructure currently an empty project 

ConfigSync (host) Program.cs wires everything together: Serilog, OpenTelemetry traces + metrics, AddApplication(), AddAdapters()

## Commands

Local environment (Postgres + mock devices):
```bash
./local-dev/local_dev_start.sh
./local-dev/local_dev_stop.sh
```

Build:
```bash
dotnet build
```

Test:
```bash
dotnet test --logger "console;verbosity=detailed"
```

List packages:
```bash
dotnet list package
```

DB schema (Atlas, manual approval required):
```bash
./local-dev/db_plan.sh    # preview
./local-dev/db_apply.sh   # apply
./local-dev/db_clean.sh   # wipe (destructive, on-demand only)
```

## API

| Method | Path | Description |
|---|---|---|
| `POST` | `/v1/executions` | Submit commands to run against a set of devices, returns a reference-id |

```bash
curl -X POST http://localhost:5000/v1/executions \
  -H "Content-Type: application/json" \
  -d '{
    "commands": ["show version", "show interfaces"],
    "deviceIds": ["device1", "device2"]
  }'
```


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