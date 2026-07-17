import Link from "next/link";
import { notFound } from "next/navigation";
import { EnvironmentsWorkspace } from "./environments-workspace";
import { ModulesWorkspace } from "./modules-workspace";
import styles from "../products.module.css";

type Product = { id: string; code: string; name: string };
type ConfigurationItem = {
  id: string;
  productId: string;
  code: string;
  name: string;
  isActive: boolean;
};

export const dynamic = "force-dynamic";

export default async function ProductDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const productResult = await fetch(
    `http://localhost:5120/api/v1/products/${id}`,
    { cache: "no-store" },
  );
  if (productResult.status === 404) notFound();
  if (!productResult.ok) throw new Error("Product API unavailable");

  const product = (await productResult.json()) as Product;
  const [moduleResult, environmentResult] = await Promise.all([
    fetch(`http://localhost:5120/api/v1/products/${id}/modules`, {
      cache: "no-store",
    }),
    fetch(`http://localhost:5120/api/v1/products/${id}/environments`, {
      cache: "no-store",
    }),
  ]);
  const modules = moduleResult.ok
    ? ((await moduleResult.json()) as { items: ConfigurationItem[] }).items
    : [];
  const environments = environmentResult.ok
    ? ((await environmentResult.json()) as { items: ConfigurationItem[] }).items
    : [];

  return (
    <main>
      <header className={styles.header}>
        <Link className={styles.back} href="/products">← Products</Link>
        <p className={styles.eyebrow}>PRODUCT · {product.code}</p>
        <h1>{product.name}</h1>
        <p>จัดการ Module และ Environment ภายใต้ Product นี้</p>
      </header>
      <ModulesWorkspace productId={id} initialModules={modules} />
      <EnvironmentsWorkspace productId={id} initialEnvironments={environments} />
    </main>
  );
}
