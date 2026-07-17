# Workflow & Traceability — Proposed

## End-to-end Flow

```text
Product / Module
  → Requirement Version
    → Test Scenario
      → Test Case Version
        → Test Cycle
          → Test Run
            → Bug
              → Build / Release
```

ความสัมพันธ์ต้องย้อนกลับได้ทั้งสองทิศทาง โดย dashboard อ่านจาก transaction จริง ไม่สร้างความสัมพันธ์จากข้อความหรือชื่อรายการ

## Requirement Workflow

```text
Draft → In Review → Approved → Implemented → Verified → Closed
          ↓              ↓
     Needs Revision   Superseded
```

Rules:

- เฉพาะ PO/Approver เปลี่ยน `In Review → Approved`
- การแก้ไข Requirement ที่ Approved ต้องสร้าง version ใหม่
- Requirement ห้าม Closed หาก acceptance criteria ที่จำเป็นยังไม่มี verification evidence
- `Superseded` ต้องอ้างอิง replacement requirement/version

## Test Case Workflow

```text
Draft → In Review → Approved → Active → Deprecated
          ↓                         ↓
     Needs Revision             Superseded
```

Rules:

- Test Cycle อ้างถึง Test Case Version ที่ immutable
- การแก้ step/expected result ของ Approved Test Case ต้องสร้าง version ใหม่
- Deprecated/Superseded case ยังต้องอ่านผลการรันย้อนหลังได้
- Clone ต้องเก็บ source test case และ source version

## Test Execution Results

Allowed result: `Not Run`, `Passed`, `Failed`, `Blocked`, `Skipped`

| Result | Actual result | Evidence | Reason/Comment |
|---|---|---|---|
| Passed | Optional | Policy-based | Optional |
| Failed | Required | Required | Required |
| Blocked | Required | Required | Required |
| Skipped | Optional | Optional | Required |

- ทุก result change เก็บ actor, timestamp และ previous value
- Re-run ไม่เขียนทับ run เดิม ให้สร้าง attempt ใหม่
- Failed run อาจมีหลาย Bug และ Bug หนึ่งรายการอาจเชื่อมหลาย run

## Bug Workflow

```text
New → Triaged → Assigned → In Progress → Fixed → Ready for Retest
 ↑                                                   ↓
 Reopened ←──────────── Retest Failed          Verified → Closed
```

Additional terminal state: `Rejected`, `Duplicate`, `Cannot Reproduce`, `Deferred`

Rules:

- `Fixed` ต้องระบุ fix build/version
- `Closed` ต้องมี verification หรือเหตุผลยกเว้นที่ผู้มีสิทธิ์อนุมัติ
- Duplicate ต้องอ้างอิง canonical bug
- Reopen ต้องบันทึกเหตุผลและ evidence ใหม่

## Build & Release Gate

Release Candidate ต้องแสดงอย่างน้อย:

- Requirement coverage และรายการข้อยกเว้น
- Execution progress แยก Passed/Failed/Blocked/Not Run
- Open defects แยก severity/priority
- Regression scope และผลล่าสุด
- Checklist, known issues และ rollback plan
- QA sign-off decision: `Approved`, `Conditional`, `Rejected`

## Integrity Rules

1. ห้าม hard delete entity ที่มี downstream traceability
2. Audit event ห้ามแก้ย้อนหลังผ่าน application API
3. Published version ต้อง immutable
4. ID ที่แสดงผู้ใช้ต้อง unique และไม่เปลี่ยนเมื่อเปลี่ยน title
5. การเปลี่ยน status ต้อง validate transition ฝั่ง server
6. Permission ต้องตรวจ product/module scope ฝั่ง server ทุกครั้ง

