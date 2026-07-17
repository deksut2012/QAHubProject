# KPI Catalog — Proposed

KPI ทุกตัวต้องมี filter ตามช่วงเวลา, Product, Module และ Release พร้อมแสดง `last calculated at` ห้ามนับข้อมูล Draft เป็น approved coverage

| KPI | Formula | Source | Frequency | Proposed target | Owner |
|---|---|---|---|---:|---|
| Approved requirement coverage | Approved requirements ที่มี ≥1 Approved/Active test case ÷ Approved requirements | Requirement/TestCaseLink | Daily | ≥90% | QA Lead |
| Execution completion | Runs ที่มี final result ÷ runs ใน cycle | TestRun latest attempt | Near real-time | 100% ก่อน sign-off | Cycle Owner |
| Pass rate | Passed final runs ÷ executed final runs | TestRun | Near real-time | ใช้ trend ไม่ตั้ง gate เดี่ยว | QA Lead |
| Evidence compliance | Failed/Blocked runs ที่ evidence ครบ ÷ Failed/Blocked runs | TestRun/Attachment | Daily | 100% | QA Lead |
| Defect leakage | Production defects ÷ defects ทั้งหมดของ release | Bug/Release | Per release | Baseline ก่อนตั้ง target | PO/QA Lead |
| Reopen rate | Reopened bugs ÷ fixed bugs ที่ retest | BugStatusHistory | Weekly | Baseline ก่อนตั้ง target | Dev Lead |
| Release readiness | Weighted gate score ตาม approved policy | Coverage/Run/Bug/Checklist | Per release | Policy-based | Release Manager |
| Active adoption | Unique active users ที่ทำ meaningful action | AuditEvent | Weekly | หลัง Pilot sizing | PO |

## Metric Definitions

- `executed final runs` = Passed + Failed + Blocked + Skipped โดยแสดง Skipped แยกเสมอ
- Pass rate ห้ามรวม Not Run และห้ามซ่อน Blocked
- Coverage ต้องผูกกับ version ที่มีผล ณ release/cycle นั้น
- Production defect ต้องมี environment classification ที่ยืนยันแล้ว
- Meaningful action ไม่รวม login/page view; รวม create, update, approve, execute และ sign-off

## Dashboard Guardrails

1. แสดง numerator/denominator เมื่อ hover หรือ drill-down
2. ทุก metric drill-down ถึงรายการต้นทางได้
3. แสดง excluded records และ data-quality warning
4. ห้ามเปรียบเทียบทีมโดยไม่ normalize scope/complexity
5. KPI เป็นสัญญาณเพื่อปรับปรุง process ไม่ใช้ลงโทษรายบุคคล

## Baseline Collection Plan

- เก็บข้อมูลย้อนหลังจากแหล่งปัจจุบัน 1–3 releases หากคุณภาพข้อมูลเพียงพอ
- หากข้อมูลย้อนหลังไม่น่าเชื่อถือ ให้ใช้ 2–4 สัปดาห์แรกของ Pilot เป็น baseline
- QA Lead ตรวจ sample calculation ก่อนเปิด dashboard ให้ผู้บริหาร
- บันทึก metric definition version เมื่อสูตรเปลี่ยน

