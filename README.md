# PropCore

Enterprise-grade multi-tenant property management platform built with .NET 10, Clean Architecture, and Domain-Driven Design.

## Overview

PropCore manages the full lifecycle of rental properties ΓÇö from onboarding organizations and listing units, through tenant leasing and rent collection, to maintenance tracking and inspections. It is designed as a scalable SaaS backend with reliable messaging and multi-tenancy.

## Architecture

```
ΓöîΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÉ
Γöé                    Api / Worker                      Γöé
Γö£ΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöñ
Γöé                   Application                        Γöé
Γöé  (CQRS ┬╖ MediatR ┬╖ FluentValidation ┬╖ Behaviors)    Γöé
Γö£ΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöñ
Γöé                     Domain                           Γöé
Γöé  (Entities ┬╖ Value Objects ┬╖ Aggregates ┬╖ Events)    Γöé
ΓööΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÿ
                          Γöé
               ΓöîΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓö┤ΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÉ
               Γöé   Infrastructure    Γöé
               Γöé  EF Core ┬╖ Redis    Γöé
               Γöé  MassTransit ┬╖ MQ   Γöé
               ΓööΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÇΓöÿ
```

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Entities, value objects, aggregates, domain events, business rules. Zero external dependencies. |
| **Application** | CQRS contracts (`ICommand`, `IQuery`), MediatR handlers, FluentValidation validators, cross-cutting behaviors (validation, logging, domain event dispatch). |
| **Infrastructure** | EF Core persistence (SQL Server), Redis caching, MassTransit + RabbitMQ messaging, ASP.NET Core Identity, file storage, transactional outbox. |
| **Api** | ASP.NET Core Web API ΓÇö Swagger, health checks, global exception handler, correlation ID middleware. |
| **Worker** | .NET Generic Host background service ΓÇö outbox processor for reliable domain event publishing. |

## Domain Model

| Aggregate | Description |
|-----------|-------------|
| **Organization** | Top-level tenant boundary. All resources are scoped to an Organization. |
| **Property** | A physical building or site (Apartment, House, Condo, Commercial, Office, Retail). |
| **Unit** | An individual rentable space within a Property. |
| **Tenant** | A renter, optionally linked to a User account. |
| **Lease** | A contract binding a Unit to a Tenant with a full lifecycle state machine. |
| **RentCharge** | A scheduled rent invoice tied to a Lease. |
| **Payment** | A payment against a RentCharge. |
| **MaintenanceRequest** | A repair/maintenance ticket for a Unit with comments and cost tracking. |
| **Inspection** | A scheduled inspection (MoveIn, MoveOut, Routine, Compliance) with checklist items and photos. |
| **Document** | Polymorphic file attachment (entity-type + entity-id). |

## Tech Stack

- **Runtime:** .NET 10, C# 14
- **ORM:** Entity Framework Core 10 (SQL Server)
- **Identity:** ASP.NET Core Identity
- **CQRS:** MediatR 12
- **Validation:** FluentValidation 11
- **Messaging:** MassTransit 8 + RabbitMQ
- **Caching:** Redis (StackExchange.Redis)
- **Logging:** Serilog
- **API Docs:** Swashbuckle / OpenAPI
- **Observability:** OpenTelemetry
- **Testing:** xUnit, FluentAssertions, Moq, Testcontainers

## Patterns

- **Domain-Driven Design** ΓÇö Aggregates, value objects, factory methods, domain events, state machines, domain exceptions
- **CQRS** ΓÇö Separate command and query models with `Result<T>` monad
- **Transactional Outbox** ΓÇö Domain events persisted in `SaveChangesAsync`, processed by the Worker and published to RabbitMQ
- **Repository & Unit of Work** ΓÇö Generic abstractions over EF Core
- **Multi-tenancy** ΓÇö Organization-scoped data across all major entities
- **Correlation ID** ΓÇö Request tracing across API and worker
- **Health Checks** ΓÇö SQL Server, Redis, and RabbitMQ connectivity

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (SDK 10.0.400+)
- SQL Server (local or remote)
- Redis
- RabbitMQ

### Configuration

Connection strings and service endpoints are in `src/PropCore.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PropCore": "Server=localhost,1433;Database=PropCore;..."
  },
  "Redis": { "ConnectionString": "localhost:6379" },
  "RabbitMQ": { "Host": "localhost", "Port": 5672, "VirtualHost": "/" },
  "Storage": { "RootPath": "uploads" }
}
```

### Run the API

```bash
cd src/PropCore.Api
dotnet run
```

API starts on `http://localhost:5212`. Swagger UI available at `/swagger`.

### Run the Worker

```bash
cd src/PropCore.Worker
dotnet run
```

The Worker polls the outbox table every 10 seconds and publishes domain events to RabbitMQ.

### Run Tests

```bash
dotnet test
```

## Project Structure

```
backend/
Γö£ΓöÇΓöÇ src/
Γöé   Γö£ΓöÇΓöÇ PropCore.Domain/            # Entities, value objects, enums, domain events
Γöé   Γö£ΓöÇΓöÇ PropCore.Application/       # CQRS contracts, handlers, validators, abstractions
Γöé   Γö£ΓöÇΓöÇ PropCore.Infrastructure/    # EF Core, Identity, Redis, MassTransit, storage
Γöé   Γö£ΓöÇΓöÇ PropCore.Api/               # ASP.NET Core Web API
Γöé   ΓööΓöÇΓöÇ PropCore.Worker/            # Background outbox processor
ΓööΓöÇΓöÇ tests/
    Γö£ΓöÇΓöÇ PropCore.Domain.Tests/
    Γö£ΓöÇΓöÇ PropCore.Application.Tests/
    Γö£ΓöÇΓöÇ PropCore.Infrastructure.Tests/
    ΓööΓöÇΓöÇ PropCore.Api.Tests/
```

## Roadmap

- [ ] Implement API controllers and CQRS handlers
- [ ] Add EF Core migrations
- [ ] Wire up OpenTelemetry tracing and metrics
- [ ] Implement message consumers in the Worker
- [ ] Add integration and end-to-end tests
- [ ] Authentication & authorization policies
- [ ] Containerization (Docker)

## License

Proprietary. All rights reserved.
