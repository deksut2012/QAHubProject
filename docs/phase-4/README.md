# Phase 4 — Test Execution

สถานะ: **Complete (conditional)**

## สิ่งที่ส่งมอบ

- Product Build และ Test Cycle ที่ผูกกับ Product, Environment และ Build
- เลือกเฉพาะ Test Case Version สถานะ Approved หรือ Active และ pin version แบบ immutable
- Cycle lifecycle: Draft → In Progress → Completed หรือ Cancelled
- Completion gate บังคับให้ Test Case ทุกรายการมีผลล่าสุดก่อนปิด Cycle
- ผล Passed, Failed, Blocked และ Skipped พร้อม validation ของ Actual Result, Evidence และ Reason
- Re-run สร้าง attempt ใหม่โดยเก็บประวัติเดิมทั้งหมด
- Evidence attachment สูงสุด 10 MB ต่อไฟล์ พร้อม metadata และ download endpoint
- Progress summary และ CSV execution report
- UI สร้าง Cycle, เลือก Test Case, เริ่ม/ปิด Cycle, บันทึกผล, re-run และแนบหลักฐาน
- Audit trail สำหรับ Build, Cycle, Item, Attempt และ Evidence

## API หลัก

- `GET/POST /api/v1/execution/builds`
- `GET /api/v1/execution/candidates?productId={id}`
- `GET/POST /api/v1/execution/cycles`
- `GET /api/v1/execution/cycles/{id}`
- `POST /api/v1/execution/cycles/{id}/start|complete|cancel`
- `POST /api/v1/execution/cycle-items/{id}/attempts`
- `POST/GET /api/v1/execution/attempts/{id}/evidence`
- `GET /api/v1/execution/cycles/{id}/report`

## การตรวจสอบ

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่าน 46 รายการ
- Web ESLint และ Next.js production build ผ่าน
- Migration: `CompleteTestExecution`

## เงื่อนไขก่อน Production

- กำหนด retention policy และ object storage สำหรับ evidence ตามปริมาณใช้งานจริง
- เพิ่ม malware scanning สำหรับไฟล์แนบ
- ยืนยัน role/approval matrix และสิทธิ์ระดับ Product กับเจ้าของกระบวนการ
