# Role & Permission Matrix — Proposed

## Roles

- **System Admin** — ตั้งค่าระบบและ user access ไม่อนุมัติเนื้อหา QA โดยปริยาย
- **Product Owner** — ดูแล Requirement และ business approval ใน Product ที่รับผิดชอบ
- **QA Lead** — อนุมัติ Test Case, Cycle, quality gate และ QA sign-off
- **QA Engineer** — ออกแบบ/รัน Test และสร้าง Bug ภายใน Product ที่ได้รับมอบหมาย
- **Developer** — อ่าน Requirement/Test, อัปเดต Bug และ fix build
- **Release Manager** — จัดการ Build/Release และ release checklist
- **Viewer/Auditor** — อ่านข้อมูลและ audit ตาม scope โดยแก้ไขไม่ได้

## Matrix

`M` Manage, `A` Approve, `E` Execute/Update assigned work, `R` Read, `—` Deny by default

| Capability | Admin | PO | QA Lead | QA | Dev | Release | Auditor |
|---|---:|---:|---:|---:|---:|---:|---:|
| User/role assignment | M | — | — | — | — | — | R |
| Product/module config | M | R | R | R | R | R | R |
| Requirement authoring | R | M/A | R | E | R | R | R |
| Test case authoring | R | R | M/A | M | R | R | R |
| Test cycle management | R | R | M/A | E | R | R | R |
| Test execution | R | R | M | E | R | R | R |
| Bug triage/close | R | R | M/A | E | E | R | R |
| Build/release | R | R | A | R | E | M | R |
| QA sign-off | R | R | A | — | — | E | R |
| Audit access | M | R | R | Own scope | Own scope | R | R |
| Security configuration | M | — | — | — | — | — | R |

## Guardrails

- Permission เป็นการรวม Role + Product Scope + Module Scope + Action
- Admin ไม่ควรอนุมัติ Requirement/Test/Release เพียงเพราะเป็น Admin
- ผู้สร้างรายการและผู้อนุมัติควรแยกคนเมื่อ policy กำหนด segregation of duties
- Sensitive actions ต้องใช้ explicit permission ไม่อนุมานจากสิทธิ์ Edit
- Export, bulk update, impersonation, permission change และ sign-off ต้อง audit
- Backend เป็น authority; การซ่อนปุ่มใน frontend ไม่ใช่ security control

## Sensitive Actions Requiring Confirmation

- เปลี่ยน role/product access
- Publish/Supersede Requirement หรือ Test Case
- Bulk import/update
- Close/Reopen Bug
- QA sign-off และ Release approval
- เปลี่ยน retention/integration/secret configuration

## Review Questions

1. องค์กรต้องการแยก Requirement Approver ออกจาก Product Owner หรือไม่?
2. QA Engineer มีสิทธิ์ close defect หรือ QA Lead เท่านั้น?
3. Developer เปลี่ยน severity/priority ได้หรือเสนอการเปลี่ยนเท่านั้น?
4. Auditor อ่าน attachment ที่ classified ได้หรือไม่?
5. ต้องมี maker-checker สำหรับ release sign-off หรือไม่?

