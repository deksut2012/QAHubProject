# Phase 8 — QA Toolbox

สถานะ: **In progress**

## สิ่งที่ส่งมอบแล้ว

- QA Toolbox hub ตาม UI Prototype
- SQL read-only safety validator โดยไม่ execute query
- SQL Query Library พร้อม permission เฉพาะ Toolbox users และ Audit Log
- ป้องกัน DML, DDL, multi-statement, comment bypass และ SELECT INTO
- Synthetic Data Generator สำหรับ JSON/CSV สูงสุด 100 รายการ
- Text/File Compare แบบรายบรรทัด
- Log Analyzer สรุป Error, Warning และ Exception group
- Data Generator, Compare และ Log Analyzer ประมวลผลใน browser ไม่ส่งข้อมูลไป server

## งานถัดไป

- SQL execution sandbox พร้อม timeout/row limit และ execution audit
- API Tester พร้อม SSRF/secret guardrail
- QR/Barcode generator และ decoder
- File compare สำหรับ CSV/Excel/JSON เชิงโครงสร้าง
- Security review และ exit-gate tests ครบทุกเครื่องมือ
