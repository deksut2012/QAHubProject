# Initial Backlog — Draft

ลำดับ P0 คือสิ่งที่ต้องปิดใน Phase 0; P1 คือ Foundation ที่เตรียมไว้สำหรับ Phase 1

| ID | Priority | Backlog item | Acceptance outcome | Owner | Status |
|---|---|---|---|---|---|
| P0-001 | P0 | แต่งตั้ง Sponsor/PO/QA Lead/Tech Lead | มีชื่อและความรับผิดชอบที่ยืนยันแล้ว | Sponsor | Open |
| P0-002 | P0 | Current workflow workshop | มี as-is flow และ pain points พร้อมหลักฐาน | QA Lead | Open |
| P0-003 | P0 | เก็บ sample templates | มี sanitized samples และ data mapping inventory | QA Lead | Open |
| P0-004 | P0 | เลือก Pilot Product | Scorecard และ decision ถูกบันทึก | PO | Baseline: ProMaxx; readiness pending |
| P0-005 | P0 | อนุมัติ MVP scope | In/Out scope มีผู้อนุมัติและวันที่ | Sponsor/PO | Open |
| P0-006 | P0 | กำหนด KPI | แต่ละ KPI มีสูตร baseline target source owner | PO/QA Lead | Open |
| P0-007 | P0 | ยืนยัน integration/security constraints | SSO, hosting, retention, network ถูกบันทึก | Tech Lead | Open |
| P0-008 | P0 | อนุมัติ architecture direction | Decisions DEC-002–005 ปิดครบ | Tech Lead | Conditional: hosting/IdP pending |
| P0-009 | P0 | Wireframe review | Core workflow มี feedback และ approval | PO/QA Lead | Open |
| P0-010 | P0 | Phase 0 exit review | Exit checklist ผ่านและลงชื่อ | Sponsor/PO | Open |
| P0-011 | P0 | Review workflow/traceability | Status และ linkage rules ผ่านการยืนยัน | QA Lead | Ready for review |
| P0-012 | P0 | Review role-permission matrix | Role boundaries และ sensitive actions ผ่าน Security review | Security | Ready for review |
| P0-013 | P0 | Review KPI catalog | สูตรและ data source ใช้งานได้จริง | PO/QA Lead | Ready for review |
| FND-001 | P1 | Scaffold frontend/backend | Build และ test ได้ในเครื่อง/CI | Tech Lead | Ready after gate |
| FND-002 | P1 | Database migration baseline | สร้าง/rollback schema ใน clean database ได้ | Backend | Ready after gate |
| FND-003 | P1 | Authentication/RBAC | Integration tests ครอบคลุม allow/deny | Backend | Ready after gate |
| FND-004 | P1 | Product/Module/Environment | Admin จัดการ master data พร้อม audit ได้ | Team | Ready after gate |
| FND-005 | P1 | Audit/error handling | Sensitive actions audited; errors use standard envelope | Team | Ready after gate |
| FND-006 | P1 | CI/CD environments | Build/test/deploy DEV และ promote UAT ได้ | DevOps | Ready after gate |
