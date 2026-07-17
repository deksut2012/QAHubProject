# QA Hub UI Guidelines

ไฟล์ `QA_Hub_UI_Prototype.html` คือ **UI source of truth** ของโครงการ

## กติกาหลัก

- ใช้ application shell แบบ sidebar 260px และ topbar 72px
- ใช้สี, radius, shadow และ spacing จาก design tokens ใน Prototype
- หัวข้อหน้าหลักประมาณ 25px; เนื้อหาและตารางเน้นความหนาแน่นสำหรับระบบงาน
- ใช้ card padding 18px และ content padding 24px บน desktop
- Forms, tables, status pills และ responsive breakpoints ต้องต่อยอดจาก pattern ใน Prototype
- หน้าใหม่ต้องอยู่ภายใน shared `AppShell` และเพิ่ม navigation ตามโครงหมวดของ Prototype
- ห้ามเปลี่ยนเป็น landing-page style หรือสร้าง visual language ใหม่โดยไม่มีเหตุผลที่บันทึกไว้

Implementation หลักอยู่ที่ `apps/web/src/app/app-shell.tsx` และ `apps/web/src/app/globals.css`
