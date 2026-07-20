# Project Statement: External Configuration Sync Service

## 1. Project Title
**ConfigSync** — A Resilient Configuration Ingestion and Serving API

## 2. Purpose
a backend service that periodically fetches configuration data from free, publicly available internet resources and persists it into a relational database, ensuring that duplicate or already-existing configuration entries are not re-inserted. The service exposes the ingested configuration through a documented REST API and produces observability signals (traces, metrics, logs) suitable for monitoring in production-like conditions.

to practice ASP.NET Core / PostgreSQL stack which creates a natural opportunity to exercise:
- Safe, idempotent data ingestion (no duplicate writes on repeated fetch cycles)
- Background/scheduled processing decoupled from HTTP request handling
- A clean read API in front of the synchronized data
- End-to-end observability of both the ingestion pipeline and the API

## 3. High-Level Solution Concept (Conceptual Only — No Implementation Detail)
1. A background process runs on a schedule (or on demand) and calls one or more free external data sources over HTTP.
2. Fetched records are normalized into a canonical configuration shape.
3. Each record is written to PostgreSQL using an upsert strategy.
4. A REST API, documented via Swagger/OpenAPI.
5. The system emits structured logs, traces, and metrics through OpenTelemetry.


## 4. Technical Scope — Areas to be Demonstrated

| Area | What the Project Must Demonstrate |
|---|---|
| ASP.NET Core (Kestrel, Middleware, Routing) | A web host with a defined middleware pipeline and clear route structure for the configuration API |
| Dependency Injection (Microsoft DI, Lifetimes) | Proper use of singleton/scoped/transient lifetimes for services such as data access, external fetch clients, and background workers |
| Async/Await (Task, ThreadPool, IO vs CPU) | Non-blocking I/O for HTTP calls and database access; clear separation of I/O-bound vs CPU-bound work |
| PostgreSQL Fundamentals (Schema, Indexing, Transactions) | A designed schema with appropriate indexes and transactional guarantees around writes |
| UPSERT / Idempotent Writes (ON CONFLICT) | Re-running ingestion must never duplicate or corrupt existing records |
| Dapper (Micro ORM, Mapping, Transactions) | Data access layer built on a micro-ORM with explicit mapping and transaction scopes, rather than a full ORM |
| Connection Pooling (Npgsql behavior) | Documented understanding of how pooled connections behave under concurrent load |
| API Design (REST, DTOs, Idempotency) | Resource-oriented endpoints, clear request/response DTOs, and idempotent write semantics at the API layer |
| Logging (Microsoft ILogger, Structured Logging) | Structured, queryable log output across ingestion and API code paths |
| Configuration (appsettings, Options pattern) | Externalized, environment-aware configuration bound via the Options pattern |
| Authentication (JWT, OAuth2, Claims) | Token-based authentication protecting non-public endpoints |
| Authorization (Policies, Roles) | Policy- and/or role-based access rules distinguishing read vs administrative operations |
| OpenTelemetry (Tracing, Metrics, Logs) | End-to-end traces across ingestion and API requests, custom metrics, and correlated logs |
| Distributed Systems Basics (Retries, Failure Handling) | Retry and backoff behavior when external sources or the database are temporarily unavailable |
| Concurrency & Threading Model | Safe concurrent execution of scheduled ingestion alongside live API traffic |
| Performance & Profiling (Latency, Throughput) | Defined expectations and a method for measuring API latency and ingestion throughput |
| HTTP Fundamentals (Request Lifecycle, Status Codes) | Correct and meaningful use of HTTP status codes and request/response semantics |
| Data Modeling (Keys, Constraints, Normalization) | A normalized schema with appropriate primary/unique keys and constraints enforcing data integrity |
| Error Handling Strategies (Retry, Fallback, Poison Messages) | A defined strategy for handling records that repeatedly fail to ingest |
| Background Processing (Hosted Services) | A supervised background process responsible for scheduled ingestion |
| Caching Basics (Optional) | Optional in-memory or distributed caching to reduce read latency for frequently requested configuration |

## 5. Functional Requirements
- FR1: The system shall fetch configuration data from one or more designated free public data sources on a defined schedule.
- FR2: The system shall only insert configuration records that do not already exist, and shall update changed records without creating duplicates.
- FR3: The system shall expose a REST API to list, filter, and retrieve individual configuration records.
- FR4: The system shall expose an interactive API specification (Swagger/OpenAPI) for all endpoints.
- FR5: The system shall protect write/administrative endpoints with authentication and authorization.
- FR6: The system shall record the outcome (success/failure/skip) of each ingestion run.

## 6. Non-Functional Requirements
- NFR1 (Observability): Every ingestion run and API request must be traceable end-to-end, with metrics available for ingestion duration, record counts, and API latency/throughput.
- NFR2 (Resilience): Transient failures from external sources or the database must be retried with backoff rather than causing data loss or crashes.
- NFR3 (Idempotency): Re-running ingestion against unchanged source data must be a no-op at the data layer.
- NFR4 (Configurability): Source endpoints, schedules, and connection settings must be externally configurable without code changes.
- NFR5 (Performance): API read paths must remain responsive under concurrent ingestion activity.

