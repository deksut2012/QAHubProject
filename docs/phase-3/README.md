# Phase 3 — Test Case Management

สถานะ: **In progress**

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

## งานถัดไป

- Clone พร้อม source traceability
- Import/Export และ duplicate detection
- Review comments, approval actor/history และ reusable templates
- Requirement coverage link management
