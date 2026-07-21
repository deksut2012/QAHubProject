# Phase 6 — Build, Release & Quality Gate

สถานะ: **In progress**

## Vertical slice แรก

- Release Candidate ผูก Product, Build, Environment และ Test Cycle
- Release notes, rollback plan และ target release date
- Release checklist พร้อม actor/timestamp
- Quality Gate อ่านข้อมูลจริงจาก Test Cycle และ Critical Bugs
- Readiness score จาก critical gates 5 รายการ
- Draft → Candidate → Approved, Conditional หรือ Rejected
- Approved sign-off ถูกบล็อกเมื่อ critical gate ไม่ผ่าน พร้อมแสดงเหตุผล
- Conditional sign-off บังคับระบุเหตุผล
- หน้า Build & Release ตาม `QA_Hub_UI_Prototype.html`
- Migration `StartReleaseQualityGate`

## Quality Gates ปัจจุบัน

- ไม่มี Critical Bug ที่ยังเปิดอยู่
- Test Cycle อยู่สถานะ Completed
- ผลล่าสุดไม่มี Failed หรือ Blocked
- Required checklist ครบ
- Release notes และ rollback plan พร้อม

## งานถัดไป

- Requirement coverage และ release scope แบบ pinned
- Known issues, deployment status และ post-release validation
- Release report/export และ sign-off audit detail
- Integration tests ให้ครบ MVP Feature Gate

## ผลตรวจล่าสุด

- .NET build ผ่านโดยไม่มี warning/error
- Automated tests ผ่าน 59 รายการ
- ESLint และ Next.js production build ผ่าน
