"use client";

import { FormEvent, useState } from "react";
import Link from "next/link";
import styles from "./products.module.css";

type Product = { id: string; code: string; name: string; isActive: boolean };
type ProductCollection = { items: Product[]; totalCount: number };

export function ProductsWorkspace({ initialProducts, initialError }: { initialProducts: Product[]; initialError: string | null }) {
  const [products, setProducts] = useState<Product[]>(initialProducts);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(initialError);
  const [code, setCode] = useState("");
  const [name, setName] = useState("");

  async function loadProducts() {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch("/backend/v1/products?pageSize=100", { cache: "no-store" });
      if (!response.ok) throw new Error("โหลดข้อมูล Product ไม่สำเร็จ");
      const data = (await response.json()) as ProductCollection;
      setProducts(data.items);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const response = await fetch("/backend/v1/products", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code, name }),
      });
      if (response.status === 409) throw new Error("Product Code นี้มีอยู่แล้ว");
      if (!response.ok) throw new Error("บันทึก Product ไม่สำเร็จ");
      setCode(""); setName("");
      await loadProducts();
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "เกิดข้อผิดพลาด");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className={styles.workspace}>
      <form className={styles.form} onSubmit={submit}>
        <div><span className={styles.step}>NEW PRODUCT</span><h2>เพิ่ม Product</h2></div>
        <label>Product Code<input value={code} onChange={(e) => setCode(e.target.value)} maxLength={32} placeholder="PMX" required /></label>
        <label>Product Name<input value={name} onChange={(e) => setName(e.target.value)} maxLength={200} placeholder="ProMaxx" required /></label>
        <button disabled={saving}>{saving ? "กำลังบันทึก..." : "เพิ่ม Product"}</button>
      </form>

      <section className={styles.list} aria-live="polite">
        <div className={styles.listHead}><div><span className={styles.step}>PRODUCT DIRECTORY</span><h2>Product ทั้งหมด</h2></div><strong>{products.length}</strong></div>
        {error && <p className={styles.error}>{error}</p>}
        {loading ? <p className={styles.muted}>กำลังโหลดข้อมูล...</p> : products.length === 0 ? <p className={styles.empty}>ยังไม่มี Product — เพิ่มรายการแรกจากแบบฟอร์มด้านซ้าย</p> : (
          <div className={styles.rows}>{products.map((product) => <Link className={styles.productLink} href={`/products/${product.id}`} key={product.id}><span className={styles.logo}>{product.code.slice(0, 2)}</span><div><strong>{product.name}</strong><p>{product.code}</p></div><span className={product.isActive ? styles.active : styles.inactive}>{product.isActive ? "Active" : "Inactive"}</span></Link>)}</div>
        )}
      </section>
    </div>
  );
}
