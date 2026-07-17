# ADR-0004: OIDC Authentication and Server-side RBAC

- Status: Proposed
- Date: 2026-07-17
- Decision owner: Security + Tech Lead (TBD)

## Context

ระบบมีข้อมูลหลาย Product และ sensitive actions จึงต้องใช้ centralized identity และ authorization ที่ตรวจทุก request

## Decision

- ใช้ OpenID Connect/OAuth 2.0 กับ enterprise Identity Provider
- API validate issuer, audience, signature, lifetime และ required claims
- Application mapping role/product/module scope อยู่ฝั่ง QA Hub
- Backend บังคับ authorization; frontend guard ใช้เพื่อ UX
- Local account อนุญาตเฉพาะเมื่อ Security อนุมัติ use case และ lifecycle control

## Consequences

- ลด password lifecycle ที่ application ต้องรับผิดชอบ
- ต้องมี IdP metadata, claim mapping และ non-production test tenant/client
- Permission changes และ access denial สำคัญต้อง audit
- ต้องออกแบบ emergency access และ service identity แยกจาก user account

