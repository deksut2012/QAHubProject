# Phase 5 — Bug Management

สถานะ: **In progress**

## Vertical slice แรก

- Bug domain พร้อม Severity, Priority และ Aging
- สร้าง Bug โดยเชื่อม Failed Test Run Attempt ได้หลายรายการ
- ตรวจสอบ traceability และ Product scope ฝั่ง server
- Workflow: New, Triaged, Assigned, In Progress, Fixed, Ready for Retest, Verified, Closed, Reopened และ terminal states
- Fixed บังคับ Fix Build, Assigned บังคับ Assignee และ Duplicate บังคับ Canonical Bug
- Reopen บังคับเหตุผล/evidence reference
- เก็บ status history และ Audit Event
- API สำหรับค้นหา, ดูรายละเอียด, สร้าง และเปลี่ยนสถานะ Bug
- หน้า Bug Kanban และฟอร์มสร้าง Bug ตาม `QA_Hub_UI_Prototype.html`
- Migration `StartBugManagement`

## งานถัดไป

- หน้ารายละเอียด Bug และ workflow controls ตามสิทธิ์
- Retest flow ที่สร้าง/เชื่อม execution attempt และปิด Bug จากผล verification
- Related Bug, comments และ evidence attachment
- Aging SLA/filter/report และ integration tests ครบ exit gate

## ผลตรวจล่าสุด

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่าน 51 รายการ
- ESLint และ Next.js production build ผ่าน
