# Architecture Decision Records

ADR บันทึกเหตุผลและผลกระทบของการตัดสินใจที่เปลี่ยนยาก สถานะ `Proposed` ต้องได้รับ Tech Lead approval ก่อนใช้เป็น Phase 1 baseline

| ADR | Decision | Status |
|---|---|---|
| [ADR-0001](adr/0001-modular-monolith.md) | Modular monolith สำหรับ MVP | Proposed |
| [ADR-0002](adr/0002-application-stack.md) | Next.js + TypeScript และ .NET 10 LTS API | Proposed |
| [ADR-0003](adr/0003-relational-database.md) | Relational database; engine selection gate | Proposed |
| [ADR-0004](adr/0004-oidc-authentication.md) | OIDC authentication และ server-side RBAC | Proposed |

## Status Lifecycle

`Proposed → Accepted → Superseded` หรือ `Proposed → Rejected`

เมื่ออนุมัติ ให้เพิ่มวันที่ ผู้ตัดสินใจ และ constraints ที่ยืนยันแล้วใน ADR นั้น
