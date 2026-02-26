# Polaris — Standardized Multimodal Ophthalmic Data Registry

A federated, privacy-preserving registry for multimodal ophthalmic imaging and clinical data. Each participating hospital operates an autonomous node; no raw patient data leaves the site.

## Technology Stack

| Layer | Technology |
|---|---|
| Frontend | Next.js 14 (App Router, TypeScript) |
| Backend | .NET Core 8 (Clean Architecture) |
| Auth | Auth0 (JWT Bearer) |
| Database | PostgreSQL 16 |
| Object Storage | MinIO |
| DICOM Server | Orthanc |
| Message Broker | RabbitMQ + MassTransit |
| Search | OpenSearch 2.x |
| Cache | Redis 7 |
| Background Jobs | Hangfire |
| Secrets | .NET User Secrets (dev) + environment variables (prod) |
| Containers | Docker Compose |

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Production-ready code; protected, requires PR + review |
| `develop` | Integration branch; auto-deploys to staging |
| `feature/*` | New features branched from `develop` |
| `hotfix/*` | Emergency fixes branched from `main` |

## Quick Start

### Prerequisites

- Docker Desktop 4.x + Docker Compose v2
- .NET 8 SDK (for EF migrations)
- Node.js 20 (for frontend development)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/ophthalmic-registry/polaris-registry.git
cd polaris-registry

# 2. Copy environment template
cp .env.example .env
# Edit .env and fill in real values before starting

# 3. Start all infrastructure services
docker compose up -d postgres redis rabbitmq minio orthanc opensearch

# 4. Apply EF Core migrations
dotnet ef database update \
  --project src/backend/OphthalmicRegistry.Infrastructure \
  --startup-project src/backend/OphthalmicRegistry.API

# 5. Start the full stack
docker compose up -d
```

The following services will be available:

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000/swagger |
| RabbitMQ Management | http://localhost:15672 |
| MinIO Console | http://localhost:9001 |
| Orthanc Web | http://localhost:8042 |
| Hangfire Dashboard | http://localhost:5000/hangfire |

## Documentation

- [Architecture Overview](docs/architecture.md)
- [Contributing Guide](CONTRIBUTING.md)
- [Security Policy](SECURITY.md)
