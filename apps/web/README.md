# QA Hub Web

Next.js + TypeScript frontend สำหรับ SeniorSoft QA Hub

## Commands

```powershell
npm.cmd install
npm.cmd run dev
npm.cmd run lint
npm.cmd run build
npm.cmd audit --audit-level=moderate
```

Development server: `http://localhost:3000`

## Rules

- ใช้ .NET API เป็น authority สำหรับ business rule และ authorization
- UI permission guard ใช้เพื่อ UX เท่านั้น
- ห้ามใส่ secret หรือ production endpoint ใน `NEXT_PUBLIC_*`
- รองรับภาษาไทย, keyboard navigation และ responsive layout
- อ่าน framework guide ใน `node_modules/next/dist/docs/` ก่อนใช้ Next.js API ที่ขึ้นกับเวอร์ชัน
