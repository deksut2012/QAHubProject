# Phase 2 — Requirement Management

สถานะ: **Implementation complete / Conditional GO**

## สิ่งที่ส่งมอบ

- Requirement stable identity และ Job Number ที่ unique ต่อ Product
- Product/Module scope, title, description, acceptance criteria และ assignee
- CRUD, pagination, search และ filter ตาม Product, Module และ Status
- Workflow: Draft → In Review → Approved → Implemented → Verified → Closed
- Revision/Supersede branches พร้อม server-side transition validation
- ห้ามแก้ Requirement หลัง Approved; การแก้ไขเนื้อหาทำได้เฉพาะ Draft/Needs Revision
- Comments พร้อม actor และ timestamp
- Attachment upload/download เก็บ metadata และ binary จำกัด 10 MB ต่อไฟล์
- Audit-backed immutable history
- CSV export ที่ใช้ filter ชุดเดียวกับรายการ
- หน้า list/create และ detail/editor ตาม `QA_Hub_UI_Prototype.html`
- Authorization deny test และ SQL Server integration coverage

## API

- `GET/POST /api/v1/requirements`
- `GET/PUT /api/v1/requirements/{id}`
- `POST /api/v1/requirements/{id}/transitions`
- `GET/POST /api/v1/requirements/{id}/comments`
- `GET/POST /api/v1/requirements/{id}/attachments`
- `GET /api/v1/requirements/{id}/attachments/{attachmentId}`
- `GET /api/v1/requirements/{id}/history`
- `GET /api/v1/requirements/export`

## Exit conditions

- Build, lint, tests, database migration และ integration tests ผ่าน
- ก่อน Production GO ต้องยืนยัน attachment retention/virus scanning policy และผูก Identity Provider จริง
- Phase ถัดไป: Phase 3 — Test Case Management
