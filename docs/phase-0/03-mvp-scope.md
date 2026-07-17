# MVP Scope — Proposed

## Scope Principle

MVP พิสูจน์ workflow หลักและ traceability ก่อนเพิ่ม Toolbox, Integration และ AI

## In Scope

1. Platform Foundation
   - Authentication, RBAC, Product, Module, Environment
   - Audit Log, error handling และ attachment policy
2. Requirement Management
   - CRUD, acceptance criteria, version, review/approval, comment/attachment
3. Test Design Repository
   - Scenario, test case, steps, test data, version, review, clone และ import/export
4. Test Cycle & Execution
   - Cycle, assignment, run result, evidence, actual result และ progress
5. Bug Management
   - Lifecycle, severity, linkage, retest และ reopen
6. Build & Release
   - Build/environment, checklist, quality gate และ QA sign-off
7. Dashboard & Reporting
   - Coverage, execution, defect และ release-readiness จากข้อมูลจริง

## Out of Scope for MVP

- SQL/API/Data Generator/File Compare/QR/Log Analyzer toolbox
- AI Test Assistant และ Error Analyzer
- Markdown repository intelligence, document diff และ smart regression
- Advanced external integrations และ automated notification
- Automation orchestration; MVP อาจเตรียม schema สำหรับ import ในอนาคต
- Mobile-first execution application

## MVP Guardrails

- Feature ใหม่ต้องเชื่อมกับ objective และมี owner
- Out-of-scope item ต้องผ่าน change control
- Dashboard ห้ามใช้ mock KPI เมื่อเข้าสู่ UAT
- Audit Log และ permission เป็น Foundation ไม่ใช่งานเสริม
- Import ต้องมี validation, preview, error report และ rollback strategy

## Pilot Acceptance

- ผู้ใช้ Pilot login และเห็นข้อมูลตามสิทธิ์
- สร้าง workflow ตั้งแต่ Requirement ถึง Release ได้ครบ
- Requirement/Test/Bug/Release มี traceability ที่ตรวจสอบได้
- Fail/Blocked บังคับ Actual Result และ Evidence
- Audit แสดงผู้แก้ไข เวลา และการเปลี่ยนสถานะสำคัญ
- KPI หลักคำนวณจาก transaction และผ่านการตรวจโดย QA Lead
- UAT critical defect ปิดครบ และมี rollback/support plan

