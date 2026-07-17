import Link from "next/link";
import styles from "./audit.module.css";

type AuditEvent = { id:string; occurredAtUtc:string; actorId:string; action:string; entityType:string; entityId:string; correlationId:string };
export const dynamic = "force-dynamic";

export default async function AuditPage(){
 let items:AuditEvent[]=[]; let error:string|null=null;
 try{const response=await fetch("http://localhost:5120/api/v1/audit-events?take=100",{cache:"no-store"});if(!response.ok)throw new Error();items=((await response.json()) as {items:AuditEvent[]}).items}catch{error="โหลด Audit History ไม่สำเร็จ"}
 return <main className={styles.page}><header><Link href="/">← QA Hub</Link><p>AUDIT & COMPLIANCE</p><h1>Audit History</h1><span>ประวัติการเปลี่ยนแปลงแบบ append-only</span></header>{error&&<div className={styles.error}>{error}</div>}<section className={styles.panel}><div className={styles.head}><h2>เหตุการณ์ล่าสุด</h2><strong>{items.length}</strong></div>{items.length===0?<p className={styles.empty}>ยังไม่มี Audit Event</p>:<div className={styles.rows}>{items.map(item=><article key={item.id}><time>{new Date(item.occurredAtUtc).toLocaleString("th-TH")}</time><div><strong>{item.entityType} · {item.action}</strong><p>{item.actorId} · {item.entityId}</p></div><code>{item.correlationId}</code></article>)}</div>}</section></main>
}
