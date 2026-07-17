# API & Audit Contract — Proposed

## API Baseline

- Base path: `/api/v1`
- JSON property: `camelCase`
- Time: ISO 8601 UTC (`2026-07-17T12:00:00Z`)
- ID: opaque UUID; display code เป็นอีก field หนึ่ง
- OpenAPI document เป็น contract และถูกตรวจใน CI
- Authentication ใช้ OIDC/OAuth2 access token หลังยืนยัน Identity Provider

## Resource Conventions

```http
GET    /api/v1/requirements/{id}
POST   /api/v1/requirements
PATCH  /api/v1/requirements/{id}
POST   /api/v1/requirements/{id}/submit-review
POST   /api/v1/requirements/{id}/approve
GET    /api/v1/requirements?productId=...&status=...&cursor=...
```

ใช้ action endpoint สำหรับ state transition ที่มี business meaning ไม่ใช้ generic status patch เพื่อข้าม validation

## Success Envelope

Single resource ส่ง resource โดยตรง พร้อม headers ที่จำเป็น ส่วน collection ใช้:

```json
{
  "items": [],
  "nextCursor": null,
  "totalCount": 0
}
```

`totalCount` อาจ omit สำหรับ query ที่มีต้นทุนสูง แต่ contract ต้องระบุชัด

## Error Contract

ใช้ `application/problem+json` ตาม Problem Details:

```json
{
  "type": "https://qahub/errors/validation",
  "title": "Validation failed",
  "status": 400,
  "code": "VALIDATION_ERROR",
  "traceId": "00-...",
  "errors": {
    "title": ["Title is required"]
  }
}
```

Rules:

- `code` เป็น machine-readable และ stable
- ห้ามส่ง stack trace, SQL, token หรือ internal exception ให้ client
- `traceId` เชื่อม client error กับ diagnostic log
- Authorization ใช้ `403`; resource ที่ต้องป้องกัน enumeration อาจตอบ `404` ตาม threat model

## Concurrency

- GET ส่ง `ETag` หรือ concurrency token
- PATCH/transition request ส่ง `If-Match`
- Conflict ตอบ `412 Precondition Failed` พร้อม code `STALE_VERSION`
- UI ต้องให้ refresh/compare ห้าม overwrite เงียบ ๆ

## Idempotency

- Create/import/sign-off ที่ client อาจ retry ต้องรองรับ `Idempotency-Key`
- Key scope ตาม actor + endpoint และมี expiration policy
- Response เดิมถูก replay เมื่อ payload fingerprint ตรงกัน

## Pagination and Filtering

- ใช้ cursor pagination สำหรับ transaction list
- Sort fields เป็น allowlist และมี deterministic tie-breaker
- Filter product/module scope ถูก intersect กับ permission ฝั่ง server
- Export เป็น background job เมื่อเกิน synchronous threshold

## Audit Event Contract

ขั้นต่ำประกอบด้วย:

| Field | Description |
|---|---|
| `eventId` | Unique immutable ID |
| `occurredAtUtc` | Server timestamp |
| `actorId` / `actorType` | User/service identity |
| `action` | Stable action code เช่น `testcase.approved` |
| `entityType` / `entityId` | Target identity |
| `productId` / `moduleId` | Authorization scope |
| `correlationId` | เชื่อม request/workflow |
| `before` / `after` | Redacted relevant changes |
| `reason` | Required สำหรับ sensitive transitions |
| `sourceIp` / `userAgent` | เก็บตาม privacy/retention policy |

## Mandatory Audit Actions

- Login failure และ access denial ที่สำคัญ
- Role/permission/product scope changes
- Publish/approve/supersede
- Test result และ evidence change
- Bug close/reopen/reject/duplicate
- Import/export/bulk change
- Release gate และ QA sign-off
- Integration/secret/retention configuration change โดยห้ามเก็บ secret value

## Logging Separation

- Diagnostic log ใช้แก้ปัญหาและมี retention สั้นกว่า
- Audit log ใช้ตรวจ accountability และมี access จำกัด
- ห้ามนำ request/response body ทั้งก้อนไป log
- Redaction ทำก่อน serialize ลง sink ทุกชนิด

