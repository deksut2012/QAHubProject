# Development Environment Assessment

- Assessment date: 2026-07-17
- Environment: Windows / PowerShell
- Purpose: ตรวจความพร้อมก่อนสร้าง Phase 1 scaffold

## Results

| Tool | Detected | Required baseline | Status |
|---|---|---|---|
| Git | 2.54.0 | Supported Git | Ready |
| Node.js | 24.16.0 | Version supported by selected Next.js release | Verify/pin during scaffold |
| npm | 11.13.0 via `npm.cmd` | Lockfile-capable npm | Ready |
| .NET SDK | 8.0.421, 10.0.302 | .NET 10 LTS SDK, latest servicing | Ready; pinned by `global.json` |
| Docker CLI | 29.5.3 | Current supported CLI | Installed |
| Docker engine | 29.5.3 | Running engine for local containers | Ready |
| SQL Server client | sqlcmd 16.0.1000.6 | Client suitable for target SQL Server | Ready; server unknown |

## Notes

- PowerShell execution policy ปิดการรัน `npm.ps1`; ใช้ `npm.cmd` ได้โดยไม่ต้องเปลี่ยน machine policy
- ห้าม scaffold API ด้วย .NET 8 แล้วค่อย upgrade เพราะ baseline เลือก .NET 10 LTS แล้ว
- Docker Desktop/engine เริ่มทำงานแล้วและตอบ `docker info` สำเร็จ
- ยังไม่มี SQL Server endpoint, edition, credentials หรือ database owner และไม่ควรบันทึก credentials ใน Git

## Prerequisites Before Scaffold

1. ระบุ SQL Server DEV endpoint/edition หรือเพิ่ม local container configuration
2. ระบุ DEV hosting/CI target
3. ระบุ Identity Provider และ non-production client registration owner

## Verification Commands

```powershell
dotnet --list-sdks
node --version
npm.cmd --version
docker info
sqlcmd -?
```

ไม่มี secret หรือ connection string ควรปรากฏในผลตรวจที่ commit เข้า repository
