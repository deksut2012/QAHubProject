# Project Charter — Draft

## Project

- ชื่อ: SeniorSoft QA Hub
- Sponsor: **TBD**
- Product Owner: **TBD**
- QA Lead: **TBD**
- Tech Lead: **TBD**
- ระยะโครงการโดยประมาณ: 9–12 เดือน
- MVP เป้าหมาย: ภายในเดือนที่ 4–5 หลังอนุมัติ Phase 0

## Problem Statement

ข้อมูล Requirement, Test Case, ผลทดสอบ, Bug และ Release มีแนวโน้มแยกอยู่หลายแหล่ง ทำให้ Traceability, Coverage, Release Readiness และ Audit ทำได้ยาก โครงการนี้จะสร้างระบบกลางที่มี workflow และข้อมูลอ้างอิงร่วมกัน

ข้อความข้างต้นเป็นสมมติฐานจากเอกสารแผน ต้องยืนยันด้วยผู้ใช้งานจริงใน Discovery workshop

## Objectives

1. เชื่อม Requirement → Test Case → Test Run → Bug → Release แบบตรวจสอบย้อนหลังได้
2. สร้าง Test Repository กลางที่รองรับ Version และ Review
3. บันทึกผลทดสอบและ Evidence อย่างเป็นมาตรฐาน
4. ใช้ Quality Gate และ QA Sign-off ประกอบการปล่อย Release
5. ให้ข้อมูล KPI มาจาก transaction จริงในระบบ

## Proposed Success Outcomes

| Outcome | วิธีวัด | Baseline | Target | Owner |
|---|---|---:|---:|---|
| Requirement coverage | Requirement ที่มี approved test / requirement ทั้งหมด | TBD | ≥ 90% | QA Lead |
| Execution traceability | Test run ที่ย้อนถึง requirement/release ได้ | TBD | 100% | QA Lead |
| Evidence completeness | Fail/Blocked ที่มี actual result และ evidence | TBD | 100% | QA Lead |
| Release visibility | Release candidate ที่ผ่าน quality-gate review | TBD | 100% | Release Owner |
| User adoption | ผู้ใช้เป้าหมาย active ต่อสัปดาห์ | TBD | กำหนดหลัง Pilot sizing | PO |

Target ทั้งหมดต้องยืนยันหลังเก็บ baseline ห้ามใช้เป็น commitment ก่อน Exit Gate

## Constraints

- ห้ามส่ง password, token, PII หรือข้อมูลลูกค้าที่ไม่ได้รับอนุญาตไปยัง AI provider
- AI output ต้องเป็น Draft และผ่าน Human Review
- Repository credential ต้องเก็บใน secret store และห้ามปรากฏใน log
- SQL Toolbox ในอนาคตต้องเริ่มจาก read-only; ห้าม AI สั่ง UPDATE/DELETE อัตโนมัติ
- Technology, hosting, SSO และ data-retention policy ยังรอการตัดสินใจ

## Governance

- Steering/Scope approval: Sponsor + PO
- Product decisions: PO
- QA workflow/data definitions: QA Lead
- Architecture/security: Tech Lead + Security
- Delivery: Dev Team + DevOps
- รอบติดตาม: Weekly project review และ decision/RAID review

