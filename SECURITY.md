# Security Policy

## Supported Versions

| Version | Supported |
|---|---|
| `main` (latest) | ✅ Active |
| `develop` | ✅ Active (pre-release) |
| Older releases | ❌ Not supported |

## Reporting a Vulnerability

**Please do NOT open a public GitHub issue for security vulnerabilities.**

Report security issues by emailing: **security@ophthalmic-registry.local**

Include in your report:

- A description of the vulnerability and its potential impact
- Steps to reproduce or a proof-of-concept (if safe to share)
- Affected component(s) and version(s)
- Any suggested mitigations

You will receive an acknowledgement within **48 hours** of submission.

## Response SLA

| Severity | Acknowledgement | Patch Target |
|---|---|---|
| Critical (CVSS ≥ 9.0) | 24 hours | 7 days |
| High (CVSS 7.0–8.9) | 48 hours | 14 days |
| Medium (CVSS 4.0–6.9) | 5 business days | 30 days |
| Low (CVSS < 4.0) | 10 business days | Next minor release |

We follow [responsible disclosure](https://en.wikipedia.org/wiki/Responsible_disclosure): we ask that you give us the time above to issue a fix before publishing details publicly.

## Out of Scope

The following are **not** considered in-scope vulnerabilities for this project:

- Vulnerabilities in third-party dependencies already reported upstream (link to the upstream advisory instead)
- Issues requiring physical access to the server
- Social engineering attacks targeting project maintainers
- Denial-of-service attacks that require a very high request rate without authentication
- Missing HTTP security headers on development-only (`localhost`) endpoints
- Self-XSS requiring the attacker to be already authenticated with equal or higher privileges

## Security Practices

- All patient identifiers are HMAC-pseudonymized before storage (see `docs/architecture.md`).
- Authentication is delegated to Auth0; no passwords are stored in this codebase.
- All API endpoints require a valid JWT; role-based authorization is enforced at the application layer.
- Secrets are managed via .NET User Secrets (development) and environment variables (production); no secrets are committed to source control.
- Audit logs are written for every data access and mutation event.
- Docker images are scanned for CVEs in CI via OWASP Dependency Check and CodeQL.
