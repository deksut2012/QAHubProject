# SeniorSoft QA Hub

ศูนย์กลางบริหารงาน QA ตั้งแต่ Requirement, Test Case, Test Execution, Bug ไปจนถึง Build และ Release

## สถานะโครงการ

- Phase ปัจจุบัน: **Phase 1 — Platform Foundation (technical scaffold)**
- Phase 0 gate: **Conditional GO** สำหรับ technical foundation; business workflow/owner approvals ยังต้องปิดก่อนเริ่ม module จริง
- เป้าหมาย Phase 0: ทำให้ Project Charter, MVP Scope, Initial Backlog, Architecture Direction และ Owner พร้อมขออนุมัติ
- เอกสารทำงาน: [docs/phase-0/README.md](docs/phase-0/README.md)

## เอกสารต้นฉบับ

- `QA_Hub_Project_Master_Plan.html` — แผนควบคุมโครงการ
- `QA_Hub_Development_Plan.html` — แผนพัฒนา
- `QA_Hub_UI_Prototype.html` — UI prototype และ developer handoff spec

## หลักการสำคัญ

- ล็อก MVP ก่อนเริ่ม Phase 1
- ออกแบบ Traceability และ Data Model ก่อน UI เชิงลึก
- ใช้ RBAC, Audit Log และ Secret Management ตั้งแต่ Foundation
- AI output ต้องเป็น Draft และผ่าน Human Review ก่อนเผยแพร่
- ห้าม commit secret, token หรือข้อมูลส่วนบุคคลลง Git

## Development

Prerequisites: .NET 10 SDK, Node.js 24, npm และ Git

```powershell
# API
dotnet run --project apps/api/QAHub.Api.csproj

# Web
npm.cmd --prefix apps/web run dev
```

Local SQL Server:

```powershell
Copy-Item .env.example .env
# แก้ placeholder ทั้งสองตำแหน่งใน .env ให้เป็นรหัสผ่าน local ที่รัดกุมและตรงกัน
docker compose up -d sqlserver

$env:ConnectionStrings__QAHub = (Get-Content .env | Where-Object { $_ -like 'ConnectionStrings__QAHub=*' }).Substring(25)
dotnet ef database update --project apps/api/QAHub.Api.csproj --startup-project apps/api/QAHub.Api.csproj
```

ไฟล์ `.env` ถูก Git ignore และห้าม commit เด็ดขาด

Quality gates:

```powershell
dotnet build QAHub.slnx
dotnet test QAHub.slnx --no-build
npm.cmd --prefix apps/web run lint
npm.cmd --prefix apps/web run build
dotnet list QAHub.slnx package --vulnerable --include-transitive
npm.cmd --prefix apps/web audit --audit-level=moderate
```

Endpoints:

- Web: `http://localhost:3000`
- API health: `http://localhost:5000/health` (พอร์ตจริงดูจาก output ของ `dotnet run`)
- API system info: `/api/v1/system/info`
