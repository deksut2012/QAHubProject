# Phase 5 — Bug Management

สถานะ: **Complete (conditional)**

## สิ่งที่ส่งมอบ

- สร้าง Bug จาก Failed Test Run Attempt และเชื่อมหลาย attempts ได้
- Severity, Priority, Aging และ SLA: Critical 1 วัน, High 3 วัน, Medium 7 วัน, Low 14 วัน
- Filter เฉพาะ SLA breach และ export Bug Aging report เป็น CSV
- Kanban workflow: New, Triaged, Assigned, In Progress, Fixed, Ready for Retest, Verified, Closed, Reopened และ terminal states
- Assigned บังคับ Assignee, Fixed บังคับ Fix Build และ Duplicate บังคับ Canonical Bug
- Verified บังคับ Passed retest attempt จาก Product เดียวกัน
- Reopen บังคับ reason/evidence reference
- Related Bugs แบบสองทิศทาง พร้อมป้องกันรายการซ้ำและการเชื่อมข้าม Product
- Comments, evidence attachment สูงสุด 10 MB, status history และ Audit Event
- หน้า Kanban, ฟอร์มสร้าง Bug และหน้ารายละเอียดตาม `QA_Hub_UI_Prototype.html`
- Migrations: `StartBugManagement`, `CompleteBugCollaboration`, `CompleteBugManagement`

## Exit gate

เส้นทาง Fail → Bug → Fix Build → Retest → Closed ทำงานครบใน domain/API และมี audit/status history โดย database integration test จะทำงานเมื่อกำหนด `QAHUB_TEST_CONNECTION`

## ผลตรวจล่าสุด

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่าน 55 รายการ
- ESLint และ Next.js production build ผ่าน

## เงื่อนไขก่อน Production

- ยืนยัน SLA จริงกับ QA Lead/Product Owner และทำให้กำหนดค่าได้ต่อ Product
- เพิ่ม malware scanning/object storage/retention policy สำหรับ evidence
- รัน database integration suite กับ SQL Server ของ CI และทำ UAT workflow ตาม role matrix
