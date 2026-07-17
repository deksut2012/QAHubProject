# ADR-0003: Relational Database

- Status: Proposed
- Date: 2026-07-17
- Decision owner: Tech Lead (TBD)

## Context

ระบบต้องการ referential integrity, versioning, transaction และ reporting ที่ชัดเจน แผนเดิมเสนอ SQL Server หรือ PostgreSQL แต่ข้อมูล licensing, hosting และ team capability ยังไม่ครบ

## Decision

ใช้ relational database ผ่าน EF Core migrations ส่วน engine เป็น decision gate:

- เลือก SQL Server หากองค์กรมี platform/operation/licensing มาตรฐานอยู่แล้ว
- เลือก PostgreSQL หากต้องการลด licensing dependency และ platform รองรับ operational requirement

ห้ามพัฒนา schema ที่พึ่ง engine-specific feature จนกว่า decision จะ Accepted

## Acceptance Inputs

- Hosting/platform support
- Backup, restore, HA และ RPO/RTO
- Licensing/TCO
- Team operation skill
- Required collation/search/reporting behavior

