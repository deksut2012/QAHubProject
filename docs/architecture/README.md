# Architecture Decision Records

ADR บันทึกเหตุผลและผลกระทบของการตัดสินใจที่เปลี่ยนยาก รายการชุดแรกได้รับการยืนยันเป็น working baseline จากคำสั่งให้ดำเนินโครงการต่อเมื่อวันที่ 2026-07-17 ข้อจำกัดที่ยังไม่ทราบถูกบันทึกแยกและห้ามอนุมานเพิ่ม

| ADR | Decision | Status |
|---|---|---|
| [ADR-0001](adr/0001-modular-monolith.md) | Modular monolith สำหรับ MVP | Accepted |
| [ADR-0002](adr/0002-application-stack.md) | Next.js + TypeScript และ .NET 10 LTS API | Accepted |
| [ADR-0003](adr/0003-relational-database.md) | SQL Server ผ่าน EF Core migrations | Accepted |
| [ADR-0004](adr/0004-oidc-authentication.md) | OIDC authentication และ server-side RBAC | Accepted; provider pending |

## Status Lifecycle

`Proposed → Accepted → Superseded` หรือ `Proposed → Rejected`

เมื่ออนุมัติ ให้เพิ่มวันที่ ผู้ตัดสินใจ และ constraints ที่ยืนยันแล้วใน ADR นั้น
