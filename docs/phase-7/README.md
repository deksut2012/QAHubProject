# Phase 7 — Dashboard & Reporting

สถานะ: **Complete**

## สิ่งที่ส่งมอบ

- Dashboard API คำนวณจาก transaction data จริงและกรองตาม Product
- Active Products, Requirements, Test Cases และ Open Bugs
- Execution progress, Pass Rate และ Passed/Failed/Blocked trend ย้อนหลัง 14 วัน
- Requirement coverage และ Requirement status visualization
- Bug severity, aging bands, SLA breach และอายุเฉลี่ย
- QA workload จาก Test Cycle assignments และ Open Bugs
- Release readiness ราย Release จาก required checklist
- Product filter และ drill-down ไปยังหน้ารายการต้นทาง
- CSV dashboard report ที่ใช้ filter เดียวกับหน้าจอ
- Scheduled report contract แบบ Daily, Weekly และ Monthly พร้อมเปิด/ปิด schedule
- Audit event สำหรับการสร้างและแก้ไข Scheduled Report
- Reconciliation tests ตรวจผลรวม latest execution กับ transaction data

## API

- `GET /api/v1/dashboard?productId={id}`
- `GET /api/v1/dashboard/report?productId={id}`
- `GET /api/v1/dashboard/schedules?productId={id}`
- `POST /api/v1/dashboard/schedules`
- `PUT /api/v1/dashboard/schedules/{id}/enabled`

## Exit Gate

**Passed** — Dashboard อ่านข้อมูลจริงจาก Requirement, Test Case, Execution, Bug และ Release โดยตรง และมี reconciliation coverage สำหรับสูตรรวมผล execution

## ผลตรวจล่าสุด

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่านทั้งหมด
- ESLint และ Next.js production build ผ่าน
