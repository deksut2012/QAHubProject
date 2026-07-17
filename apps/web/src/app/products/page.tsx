import Link from "next/link";
import { ProductsWorkspace } from "./products-workspace";
import styles from "./products.module.css";

type Product = { id: string; code: string; name: string; isActive: boolean };

export const dynamic = "force-dynamic";

export default async function ProductsPage() {
  let initialProducts: Product[] = [];
  let initialError: string | null = null;
  try {
    const response = await fetch("http://localhost:5120/api/v1/products?pageSize=100", { cache: "no-store" });
    if (!response.ok) throw new Error();
    const data = (await response.json()) as { items: Product[] };
    initialProducts = data.items;
  } catch {
    initialError = "เชื่อมต่อ QA Hub API ไม่สำเร็จ";
  }

  return (
    <main className={styles.page}>
      <header className={styles.header}>
        <div>
          <Link href="/" className={styles.back}>← QA Hub</Link>
          <p className={styles.eyebrow}>PLATFORM CONFIGURATION</p>
          <h1>Products</h1>
          <p>จัดการ Product หลักและเตรียมโครงสร้าง Module สำหรับงาน QA</p>
        </div>
      </header>
      <ProductsWorkspace initialProducts={initialProducts} initialError={initialError} />
    </main>
  );
}
