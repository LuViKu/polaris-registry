# Contributing to Polaris — Ophthalmic Data Registry

Thank you for contributing! Please read this guide carefully before opening a pull request.

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Protected; production-ready. Requires PR + at least one code review. |
| `develop` | Integration branch; automatically deployed to staging on push. |
| `feature/<ticket>-short-description` | New work; always branch from `develop`. |
| `hotfix/<ticket>-short-description` | Emergency patches; branch from `main`, merge back to both `main` and `develop`. |

## Pull Request Process

1. Fork the repository (external contributors) or create a branch (team members).
2. Branch from `develop` (or `main` for hotfixes).
3. Keep PRs focused — one logical change per PR.
4. Fill in the PR template completely, including the **GDPR Review Gate**.
5. Ensure all CI checks pass before requesting review.
6. At least one reviewer from `@ophthalmic-registry/core-team` must approve.
7. PRs to `main` additionally require a GDPR sign-off comment from a designated data-protection reviewer.

### PR Checklist

- [ ] Code compiles and all tests pass locally
- [ ] Unit tests added for new business logic
- [ ] No hardcoded credentials or patient data
- [ ] GDPR Review Gate section completed
- [ ] Documentation updated if public APIs changed

## Coding Standards

### .NET / C#

- Follow **Clean Architecture** — Domain → Application → Infrastructure → API.
- Apply **SOLID** principles; prefer composition over inheritance.
- Use meaningful, intention-revealing names; avoid abbreviations.
- Add **XML doc comments** (`/// <summary>`) on all `public` types and members.
- Validate inputs with **FluentValidation** in the Application layer.
- All commands and queries go through **MediatR**; no direct repository calls from controllers.
- `async`/`await` all I/O; never use `.Result` or `.Wait()`.

### TypeScript / Next.js

- Enable and maintain **strict mode** in `tsconfig.json`.
- All code must pass **ESLint** (`next lint`) with zero warnings.
- Apply **Prettier** formatting before committing (`npx prettier --write .`).
- Prefer React Server Components; use `"use client"` only where necessary.
- Validate all form inputs and API responses with **Zod** schemas.

## GDPR Rules

> **Non-negotiable.** Any PR that violates these rules will be rejected immediately.

1. **Never commit real patient data** — not even anonymized exports used as fixtures.
2. **Pseudonymize all test data** — use the `PseudonymizationService` to generate synthetic identifiers.
3. **Document data flows** — if a PR introduces a new field that could be PII, update `docs/data-flows.md`.
4. **Minimize data** — only collect and store data required for the stated clinical purpose.
5. **Reviewers must check PII handling** — comment `GDPR: approved` in the PR when satisfied.
6. **Audit logging** — every new endpoint that reads or mutates patient data must emit an `AuditLog` entry.

## Commit Message Format

This project uses [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short summary>

[optional body]

[optional footer(s)]
```

**Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `ci`, `perf`

**Examples:**

```
feat(patients): add pseudonymized patient search endpoint
fix(imaging): correct DICOM modality filter in Orthanc client
docs(arch): update data-flow diagram with OpenSearch indexing
chore(deps): bump Npgsql.EntityFrameworkCore.PostgreSQL to 8.0.4
```

## Testing Requirements

| Layer | Requirement |
|---|---|
| Domain entities | Unit tests for all factory methods and invariants |
| Application handlers | Unit tests with mocked repositories |
| Infrastructure | Integration tests against real PostgreSQL (CI spins up a container) |
| API controllers | Integration tests using `WebApplicationFactory` |
| Frontend components | Component tests with React Testing Library |
| End-to-end | Playwright smoke tests for critical user flows |

Run backend tests:
```bash
dotnet test src/backend/OphthalmicRegistry.sln
```

Run frontend checks:
```bash
cd src/frontend && npm run lint && npm run type-check && npm run build
```
