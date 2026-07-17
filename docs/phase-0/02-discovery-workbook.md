# Discovery Workbook

## Workshop Plan

| Workshop | ผู้เข้าร่วมขั้นต่ำ | Output | สถานะ |
|---|---|---|---|
| Business goals & pain points | Sponsor, PO, QA Lead | Goal, priority, success criteria | Pending |
| Current QA workflow | QA Lead, QA users, Dev representative | As-is flow, handoff, bottleneck | Pending |
| Product & pilot selection | PO, QA Lead, Product owner | Pilot scorecard | Pending |
| Roles & permission | PO, QA Lead, Security | Role/permission matrix | Pending |
| Templates & data migration | QA users, Data owner | Sample files, mapping, volume | Pending |
| Integration & infrastructure | Tech Lead, DevOps, Security | SSO, hosting, network, retention | Pending |

## Evidence Request

- ตัวอย่าง Requirement และ Acceptance Criteria อย่างน้อย 5 รายการ
- Test Case template และข้อมูลตัวอย่างที่ลบ PII แล้ว
- ตัวอย่าง Test Cycle/Test Report
- Bug workflow, severity/priority definition และตัวอย่าง defect
- Build/Release checklist และ QA sign-off ปัจจุบัน
- Product/Module list, user role list และ environment list
- Integration inventory เช่น SSO, Git, notification และ issue tracker
- ปริมาณข้อมูลโดยประมาณและ retention requirement

ห้ามนำ production secret หรือข้อมูลลูกค้าที่ระบุตัวบุคคลได้มาเก็บใน repository นี้

## Current-state Interview Questions

1. งานเริ่มจากใครและข้อมูลต้นทางอยู่ที่ใด?
2. จุดใดต้องคัดลอกข้อมูลหรือทำซ้ำด้วยมือ?
3. จุดใดทำให้หา coverage, owner หรือสถานะล่าสุดไม่ได้?
4. Test Case ถูก review/version อย่างไร?
5. Fail/Blocked ต้องแนบหลักฐานอะไร?
6. Bug เชื่อมกลับ Test Run และ Release อย่างไร?
7. ใครมีสิทธิ์ approve, reopen, sign-off หรือ publish?
8. รายงานใดใช้ตัดสินใจ release และเชื่อถือได้เพียงใด?

## Pilot Scorecard

ให้คะแนน 1–5 หลัง workshop

| เกณฑ์ | น้ำหนัก | ProMaxx | MyMaxx2 |
|---|---:|---:|---:|
| Business value | 25% | TBD | TBD |
| ทีมพร้อมร่วม Pilot | 20% | TBD | TBD |
| มีข้อมูลตัวอย่างพร้อม | 20% | TBD | TBD |
| Workflow ครอบคลุม MVP | 20% | TBD | TBD |
| Integration complexity เหมาะสม | 15% | TBD | TBD |

ข้อเสนอเริ่มต้น: เลือก Pilot หลักเพียง 1 Product เพื่อลด scope; ใช้อีก Product เป็น validation หลัง workflow หลักเสถียร

## RAID Log

| ID | Type | รายการ | ผลกระทบ | Owner | Due | Status |
|---|---|---|---|---|---|---|
| R-001 | Risk | MVP ใหญ่เกินเวลาที่กำหนด | Timeline/quality | PO | Exit Gate | Open |
| R-002 | Risk | Data model ถูกกำหนดจาก UI ก่อน traceability | Rework | Tech Lead | Exit Gate | Open |
| A-001 | Assumption | ทีมต้องการระบบกลางแทนข้อมูลหลายแหล่ง | Scope | PO | Workshop 1 | Validate |
| A-002 | Assumption | Phase 1–7 เป็น MVP | Scope | Sponsor/PO | Exit Gate | Validate |
| D-001 | Dependency | ต้องมี sample template และ workflow จริง | Discovery | QA Lead | Workshop 2 | Open |
| D-002 | Dependency | ต้องทราบ SSO/hosting/security constraints | Architecture | Tech Lead | Workshop 6 | Open |

## Decision Log

| ID | Decision | Options | Owner | Due | Status |
|---|---|---|---|---|---|
| DEC-001 | Pilot Product | ProMaxx / MyMaxx2 | PO + QA Lead | Exit Gate | Pending |
| DEC-002 | Frontend | Next.js / React SPA | Tech Lead | Exit Gate | Pending |
| DEC-003 | Database | SQL Server / PostgreSQL | Tech Lead | Exit Gate | Pending |
| DEC-004 | Authentication | Enterprise SSO / local identity | Security + Tech Lead | Exit Gate | Pending |
| DEC-005 | Hosting | On-prem / cloud / hybrid | Sponsor + DevOps | Exit Gate | Pending |

