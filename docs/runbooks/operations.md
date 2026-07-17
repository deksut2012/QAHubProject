# QA Hub Operations Runbook

## Local/DEV Deployment

1. Copy `.env.example` to `.env` and replace both password placeholders.
2. Run `docker compose -f compose.yaml -f compose.dev.yaml up -d --build`.
3. Apply migrations from a controlled release job: `dotnet ef database update --project apps/api/QAHub.Api.csproj`.
4. Verify `/health`, `/api/v1/session`, Web `/products`, `/audit` and `/admin`.

Outside Development, `Authentication:Authority` and `Authentication:Audience` are mandatory. The Development authentication handler must never be enabled in UAT/Production.

## Backup

Run `deploy/scripts/backup-sql.ps1`. Backups stay in the SQL persistent volume. Copy them to approved encrypted storage and apply the organizational retention policy.

## Restore Drill

`restore-sql.ps1` verifies checksum only by default. Full restore requires an approved maintenance window, a separate target database, logical file mapping, application smoke tests and recorded evidence. Never overwrite the active database as an ad-hoc test.

## Recovery Targets

RPO/RTO remain organizational decisions. Record agreed targets, backup frequency, retention, encryption owner and quarterly restore-drill evidence before Production.
