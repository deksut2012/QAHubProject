# Phase 1 Readiness Plan

เอกสารนี้เตรียม execution หลัง Phase 0 Exit Gate ผ่าน ห้ามถือว่าอนุมัติให้เริ่ม Phase 1 โดยอัตโนมัติ

## Entry Criteria

- Phase 0 Exit Gate เป็น GO หรือ Conditional GO ที่ไม่มี architecture blocker
- DEC-001 ถึง DEC-004 accepted และ DEC-005 มี DEV target ที่ยืนยันแล้ว
- มีชื่อ Tech Lead, repository owner และ environment owner
- ยืนยัน supported browser, hosting, SSO และ database
- มี DEV environment/network access ที่จำเป็น
- มี Definition of Done และ branch/PR policy

## Recommended Implementation Order

1. Repository conventions, solution/workspace และ CI build
2. Backend/Frontend health page และ environment configuration
3. Database migration baseline
4. Authentication integration
5. Server-side authorization/RBAC
6. Product/Module/Environment configuration
7. Audit event pipeline และ standard error contract
8. Integration tests และ DEV deployment

## Foundation Deliverables

| Deliverable | Verification |
|---|---|
| Frontend application | Lint/typecheck/unit test/build ผ่าน CI |
| .NET API | Build/unit/integration test ผ่าน CI |
| Database migrations | Apply และ rollback/test on clean database |
| Authentication | Valid/expired/invalid token scenarios ผ่าน |
| RBAC | Allow/deny tests ครบ role + product scope |
| Master configuration | CRUD + validation + audit integration tests |
| Error handling | Problem Details และ correlation ID contract tests |
| Audit | Sensitive actions produce redacted immutable events |
| DEV deployment | Health/readiness checks และ smoke test ผ่าน |

## Repository Structure — Proposed

```text
/
├── apps/
│   ├── web/                 # Next.js + TypeScript
│   └── api/                 # .NET 10 Web API
├── tests/
│   ├── integration/
│   └── architecture/
├── deploy/
├── docs/
│   ├── architecture/
│   └── phase-0/
├── .editorconfig
├── Directory.Build.props
└── README.md
```

## Quality Gates

- No secret ใน Git history หรือ build artifact
- Compiler warnings policy ถูกกำหนดและบังคับ
- Dependency vulnerability scan ไม่มี unresolved critical finding
- Unit/integration tests ผ่าน
- Database migration ถูกทดสอบบน clean instance
- Authorization deny cases มี test ไม่ใช่เฉพาะ happy path
- OpenAPI generation ไม่มี breaking drift ที่ไม่ได้อนุมัติ
- Container/process รันด้วย least privilege

## Deferred Until Decision

- สร้าง source scaffold: architecture decisions พร้อม แต่ต้องติดตั้ง .NET 10 SDK
- สร้าง deployment pipeline: รอ CI/CD และ hosting platform
- เลือก database-specific concurrency/index strategy: ใช้ SQL Server baseline; รอ edition/collation/hosting
- ตั้งค่า SSO metadata/secret: รอ Identity Provider และ security owner
