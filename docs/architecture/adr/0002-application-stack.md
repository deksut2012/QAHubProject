# ADR-0002: Application Stack

- Status: Accepted
- Date: 2026-07-17
- Decision authority: Project working baseline, 2026-07-17

## Context

แผนเดิมเสนอ React/Next.js, TypeScript และ .NET 8 Web API แต่ .NET 8 จะสิ้นสุดการรองรับวันที่ 10 พฤศจิกายน 2026 จึงไม่เหมาะเป็น baseline ของโครงการใหม่ที่มีแผนส่งมอบหลายเดือน

## Decision

- Web: Next.js + TypeScript โดยใช้ framework routing/layout แต่ business API อยู่ใน .NET API
- API: .NET 10 LTS Web API พร้อม OpenAPI และ Problem Details
- UI component library เลือกหลังตรวจ license, accessibility และ prototype fit

## Consequences

- Type safety และ ecosystem เหมาะกับ data-heavy web application
- .NET รองรับ enterprise integration และ validation/testing tooling
- ต้องกำหนด boundary ไม่ให้ business rule กระจายระหว่าง Next.js server layer กับ .NET API
- .NET 10 LTS รองรับถึงเดือนพฤศจิกายน 2028 แต่ยังต้องใช้ servicing patch ล่าสุดตาม support policy
- ต้อง pin Node major ที่รองรับและบันทึก runtime versions ใน repository ก่อน Phase 1

## References

- Microsoft .NET releases and support: https://learn.microsoft.com/en-us/dotnet/core/releases-and-support
- Microsoft .NET support policy: https://dotnet.microsoft.com/en-us/platform/support/policy
- Next.js installation requirements: https://nextjs.org/docs/pages/getting-started/installation
