"use client";

import { FormEvent, useState } from "react";
import styles from "../products.module.css";

type ProductEnvironment = {
  id: string;
  productId: string;
  code: string;
  name: string;
  isActive: boolean;
};

export function EnvironmentsWorkspace({
  productId,
  initialEnvironments,
}: {
  productId: string;
  initialEnvironments: ProductEnvironment[];
}) {
  const [items, setItems] = useState(initialEnvironments);
  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const response = await fetch(
        `/backend/v1/products/${productId}/environments`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ code, name }),
        },
      );
      if (response.status === 409) throw new Error("Environment Code นี้มีอยู่แล้ว");
      if (!response.ok) throw new Error("บันทึก Environment ไม่สำเร็จ");
      const created = (await response.json()) as ProductEnvironment;
      setItems((current) =>
        [...current, created].sort((a, b) => a.code.localeCompare(b.code)),
      );
      setCode("");
      setName("");
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "เกิดข้อผิดพลาด");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className={styles.workspace}>
      <form className={styles.form} onSubmit={submit}>
        <div><span className={styles.step}>NEW ENVIRONMENT</span><h2>เพิ่ม Environment</h2></div>
        <label>Environment Code<input value={code} onChange={(event) => setCode(event.target.value)} maxLength={32} placeholder="UAT" required /></label>
        <label>Environment Name<input value={name} onChange={(event) => setName(event.target.value)} maxLength={200} placeholder="User Acceptance Test" required /></label>
        <button disabled={saving}>{saving ? "กำลังบันทึก..." : "เพิ่ม Environment"}</button>
      </form>
      <section className={styles.list} aria-live="polite">
        <div className={styles.listHead}><div><span className={styles.step}>ENVIRONMENT DIRECTORY</span><h2>Environment ทั้งหมด</h2></div><strong>{items.length}</strong></div>
        {error && <p className={styles.error}>{error}</p>}
        <div className={styles.rows}>{items.map((item) => <article key={item.id}><span className={styles.logo}>{item.code.slice(0, 2)}</span><div><strong>{item.name}</strong><p>{item.code}</p></div><span className={item.isActive ? styles.active : styles.inactive}>{item.isActive ? "Active" : "Inactive"}</span></article>)}</div>
        {items.length === 0 && <p className={styles.empty}>ยังไม่มี Environment</p>}
      </section>
    </div>
  );
}
