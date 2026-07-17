# ADR-0001: Modular Monolith for MVP

- Status: Proposed
- Date: 2026-07-17
- Decision owner: Tech Lead (TBD)

## Context

MVP มี domain ที่เกี่ยวข้องกันและต้องรักษา transaction/traceability ขณะที่ทีม ขนาดโหลด และ scaling bottleneck ยังไม่ทราบ การเริ่มด้วย distributed services จะเพิ่ม deployment, consistency และ observability cost ก่อนมีหลักฐานความจำเป็น

## Decision

ใช้ modular monolith สำหรับ API โดยแยก module boundary, dependency direction และ data ownership ชัดเจน Modules สื่อสารผ่าน application contract/event ภายใน ห้ามเข้าถึง implementation หรือ table ของ module อื่นโดยไม่มี contract

## Consequences

- เริ่มพัฒนาและ deploy ง่ายกว่า microservices
- Transaction consistency ของ core workflow ตรงไปตรงมา
- ต้องมี architecture tests ป้องกัน boundary erosion
- สามารถแยก service ภายหลังเมื่อมี scaling/ownership evidence

