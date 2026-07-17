# Phase 2 — Requirement Management

สถานะ: **In progress**

## Vertical slice แรก

- Requirement stable identity และ Job Number ที่ unique ต่อ Product
- Product/Module scope, title, description, acceptance criteria และ assignee
- Workflow: Draft → In Review → Approved → Implemented → Verified → Closed พร้อม Needs Revision/Superseded
- Server-side transition validation และการห้ามแก้ Requirement หลัง Approved
- CRUD/filter API, status transition API และ audit-backed history API
- หน้ารายการ, filter และสร้าง Requirement ตาม UI Prototype

## งานถัดไป

- Requirement detail/editor และ action เปลี่ยนสถานะใน UI
- Comments และ attachment metadata/storage integration
- Export และ advanced filtering
- Product/module scoped authorization tests
