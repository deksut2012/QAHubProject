# ADR-0003: Relational Database

- Status: Accepted
- Date: 2026-07-17
- Decision authority: Project working baseline, 2026-07-17

## Context

ระบบต้องการ referential integrity, versioning, transaction และ reporting ที่ชัดเจน แผนเดิมเสนอ SQL Server หรือ PostgreSQL สภาพแวดล้อมปัจจุบันมี SQL Server tooling และ Product Pilot มี SQL Server อยู่ใน technology landscape

## Decision

ใช้ SQL Server ผ่าน EF Core migrations เป็น baseline ของ MVP โดยยังคง domain model ที่ไม่ผูก stored procedure/vendor feature โดยไม่มี ADR เพิ่มเติม

## Constraints to Confirm

- SQL Server edition และ licensing
- Hosting/platform endpoint
- Backup, restore, HA และ RPO/RTO
- Team operation skill
- Required collation/search/reporting behavior
