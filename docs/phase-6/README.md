# Phase 6 — Build, Release & Quality Gate

สถานะ: **Complete (conditional)**

## สิ่งที่ส่งมอบ

- Release Candidate ผูก Product, Build, Environment และ Test Cycle
- Requirement release scope แบบ pinned และ Known Issues ที่อ้าง Bug จริง
- Release notes, rollback plan, target release date และ checklist พร้อม actor/timestamp
- Readiness score และ Quality Gate จาก transaction จริง
- Draft → Candidate → Approved, Conditional หรือ Rejected
- Approved sign-off ถูกบล็อกพร้อมเหตุผลเมื่อ Critical Gate ไม่ผ่าน
- Conditional sign-off บังคับเหตุผล
- Deployment status และ post-release validation ก่อนเปลี่ยนเป็น Released
- CSV Release Report รวม gate, checklist, sign-off และ deployment
- Audit ครอบคลุม Release, checklist, requirement scope และ known issues
- หน้า Build & Release ตาม `QA_Hub_UI_Prototype.html`
- Migrations: `StartReleaseQualityGate`, `CompleteReleaseQualityGate`

## Critical Quality Gates

- ไม่มี Critical Bug ที่ยังเปิดอยู่
- Test Cycle อยู่สถานะ Completed
- ผลล่าสุดไม่มี Failed หรือ Blocked
- Requirement ใน release scope เป็น Verified หรือ Closed ครบ
- Required checklist ครบ
- Release notes และ rollback plan พร้อม

## Exit gate

ระบบบล็อก Approved sign-off เมื่อ Critical Gate ไม่ผ่านและตอบกลับเหตุผลราย gate โดย integration scenario ครอบคลุม Requirement → Test → Bug/Fix → Release → Sign-off → Deployment

## ผลตรวจล่าสุด

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่าน 61 รายการ
- ESLint และ Next.js production build ผ่าน

## เงื่อนไขก่อน Production

- ยืนยันสูตร readiness/exception authority กับ QA Lead และ Release Manager
- รัน database integration suite ใน CI ด้วย `QAHUB_TEST_CONNECTION`
- ทำ UAT sign-off และทดสอบ rollback/post-release checklist กับ Pilot Product
