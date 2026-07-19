# Phase 3 — Test Case Management

สถานะ: **Implementation complete / Conditional GO**

## Vertical slice แรก

- Stable Test Case ID ที่ unique ต่อ Product
- Scenario, Preconditions, Tags, Test Steps, Test Data และ Expected Result
- Versioned content และ current version pointer
- Review workflow: Draft → In Review → Approved → Active → Deprecated
- Needs Revision และ Superseded branches พร้อม server-side validation
- Approved/Active version immutable และสร้าง Draft version ใหม่จาก snapshot เดิมได้
- Filter ตาม Product, Requirement, Status และข้อความค้นหา
- หน้า list/create/detail/editor ตาม UI Prototype
- Clone Test Case พร้อมคัดลอกเนื้อหาและ steps ไปยัง Test Case ใหม่
- Clone traceability สำหรับเก็บ source test case/version ของการ clone
- Duplicate detection จาก title/scenario แบบ normalized
- Review comments และ approval/status history พร้อม actor/timestamp
- CSV import/export API และ reusable templates ในหน้าสร้าง Test Case
- Requirement coverage link management

## Exit conditions

- Build, lint, tests และ SQL Server migration/integration tests ผ่าน
- ก่อน Production GO ต้องยืนยันสิทธิ์ผู้อนุมัติ, รูปแบบ CSV มาตรฐาน และ template ownership
- Phase ถัดไป: Phase 4 — Test Execution
