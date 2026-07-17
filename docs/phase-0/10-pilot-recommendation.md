# Pilot Product Recommendation

## Recommendation

เสนอให้ใช้ **ProMaxx เป็น Primary Pilot** และใช้ **MyMaxx2 เป็น Secondary Validation** หลัง core workflow เสถียร

สถานะ: **Accepted as working baseline — ต้องยืนยันทีมและข้อมูลพร้อมก่อนเริ่ม Pilot**

## Rationale

จากข้อมูลใน Project Plan, ProMaxx ครอบคลุม Desktop, Database, Stock, Cost, Permission, Multi-Branch, Build และ Regression จึงเหมาะกับการพิสูจน์ workflow หลักและ traceability ของ MVP โดยยังไม่ดึงความซับซ้อน Web/Mobile/API/Payment เข้ามาพร้อมกัน

MyMaxx2 เหมาะเป็น validation ลำดับถัดไปเพื่อพิสูจน์ว่า data model และ UX รองรับ Web, Mobile, API และ Integration โดยไม่ออกแบบระบบให้ผูกกับ Desktop workflow เท่านั้น

## Provisional Score

คะแนนต่อไปนี้เป็น hypothesis จากเอกสาร ไม่ใช่ผล workshop

| เกณฑ์ | น้ำหนัก | ProMaxx | MyMaxx2 | หมายเหตุ |
|---|---:|---:|---:|---|
| Business/workflow coverage | 25% | 5 | 4 | ProMaxx ครอบคลุม QA workflow หลักหลายมิติ |
| Team readiness | 20% | TBD | TBD | ต้องยืนยัน owner และผู้ใช้ Pilot |
| Sample data readiness | 20% | TBD | TBD | ต้องตรวจ template/test data |
| MVP fit | 20% | 5 | 4 | MyMaxx2 เพิ่ม API/payment complexity |
| Manageable integration | 15% | 4 | 3 | เป็นสมมติฐานรอยืนยัน architecture |

ห้ามสรุป weighted winner จนกว่าคะแนน Team readiness และ Sample data readiness จะครบ

## Entry Criteria

- มี Product Owner, QA champion และ Dev representative
- มี sanitized Requirement/Test Case/Bug/Release samples
- มี release candidate หรือ workflow จริงที่ใช้ทดสอบ Pilot ได้
- ระบุ user group, environment และ support contact
- ไม่มี blocker ด้านข้อมูลหรือสิทธิ์ที่ไม่มี owner

## Pilot Boundaries

- เริ่ม 1–2 Modules ที่มี workflow ครบและทีมพร้อม
- จำกัด user cohort แล้วขยายเมื่อ critical workflow ผ่าน
- ไม่รวม Toolbox/AI/advanced integration ใน Primary Pilot
- ใช้ feedback log และ weekly checkpoint
- กำหนด rollback/export plan ก่อน migrate ข้อมูลจริง

## Decision Required

- Primary Pilot Product และ Module
- Pilot owner และ user cohort
- วันที่เริ่ม/สิ้นสุด
- Sample data owner
- Success threshold และ go/no-go authority
