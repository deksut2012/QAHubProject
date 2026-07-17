# Phase 1 — Platform Foundation

สถานะ: **Implementation complete / Conditional GO**

## สิ่งที่ส่งมอบ

- Web application ด้วย Next.js และ API ด้วย ASP.NET Core
- SQL Server, Entity Framework migrations และ database integration test
- Product, Module และ Environment management
- Authentication แบบ Development identity และ JWT/OIDC สำหรับ environment จริง
- RBAC policies, User/Role administration และ session endpoint
- Transactional audit events พร้อม API และหน้าจอตรวจสอบ
- Problem Details error contract พร้อม trace ID
- CI quality gates, Docker images และ DEV Compose deployment
- Runbook สำหรับ deploy, migration, backup และ restore verification

## Quality gates

- `dotnet build QAHub.slnx`
- `dotnet test QAHub.slnx --no-build`
- `npm.cmd --prefix apps/web run lint`
- `npm.cmd --prefix apps/web run build`
- dependency vulnerability audit
- SQL Server migration/integration test ผ่านตัวแปร `QAHUB_TEST_CONNECTION`
- Docker image build และ Compose configuration validation

## เงื่อนไขก่อน Production GO

- กำหนด `Authentication:Authority` และ `Authentication:Audience` จาก Identity Provider จริง
- สร้าง GitHub Actions secret `QAHUB_CI_SA_PASSWORD`
- จัดเก็บ runtime secrets ใน secret manager ของ environment ห้าม commit ลง Git
- ซ้อม restore จาก backup จริง และบันทึกค่า RPO/RTO
- ให้ Security และ Operations อนุมัติ deployment checklist

รายละเอียดการปฏิบัติงานอยู่ที่ [Operations runbook](../runbooks/operations.md)
