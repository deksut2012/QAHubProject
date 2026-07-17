# Architecture Direction — Proposed

เอกสารนี้กำหนด direction เพื่อประมาณงานและออกแบบ backlog ยังไม่ใช่ Architecture Decision Record ที่อนุมัติแล้ว

## Proposed Baseline

- Frontend: Next.js + TypeScript
- Backend: .NET 8 Web API
- Database: เลือก SQL Server หรือ PostgreSQL หลังยืนยัน hosting/licensing/team skill
- API style: REST, OpenAPI-first, versioned endpoints
- Deployment: containerized application แยก DEV/UAT/PROD
- Observability: structured log, correlation ID, health check, metrics และ audit แยกจาก diagnostic log
- File storage: object/file storage พร้อม malware scan, size/type policy และ metadata ใน database

## Logical Boundaries

- Identity & Access
- Product Configuration
- Requirement & Traceability
- Test Design
- Test Execution
- Defect Management
- Build & Release
- Reporting
- Administration & Audit

เริ่มเป็น modular monolith เพื่อให้ transaction และ deployment ไม่ซับซ้อน แต่บังคับ module boundary เพื่อรองรับการแยก service หากมีเหตุผลในอนาคต

## Data Principles

- ใช้ immutable history/version สำหรับ Requirement และ Test Case ที่ publish แล้ว
- เก็บ audit ของคำสั่งสำคัญแบบ append-only
- ใช้ soft delete เฉพาะ entity ที่นโยบายอนุญาต และต้องไม่ทำลาย traceability
- ใช้ UTC ใน storage และแสดงผลตาม timezone ผู้ใช้
- นิยาม ID, status transition และ referential integrity ก่อนทำ UI เชิงลึก
- Attachment ต้องมี owner, classification, checksum และ retention metadata

## Security Baseline

- Enterprise SSO เป็นตัวเลือกแนะนำ แต่ต้องยืนยัน provider
- Authorization ฝั่ง server ทุก endpoint; UI guard ใช้เพื่อ UX เท่านั้น
- Principle of least privilege และ product/module scope
- Secret อยู่ใน secret manager ไม่อยู่ใน source, config ที่ commit หรือ log
- Upload allowlist, malware scanning และ download authorization
- Rate limit, input validation, secure headers และ dependency scanning
- กำหนด audit retention, backup/restore และ incident owner ก่อน Production

## Decisions Required Before Phase 1

1. Hosting topology และ network constraint
2. Identity provider/SSO protocol
3. Database engine และ HA/backup requirement
4. Attachment storage และ maximum size
5. Data classification/retention
6. CI/CD platform และ environment approval flow
7. Browser support และ accessibility target

