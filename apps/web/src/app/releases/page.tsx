import { ReleaseWorkspace } from "./workspace";
type Product = { id: string; code: string; name: string };
export const dynamic = "force-dynamic";
export default async function Page() {
  let products: Product[] = [],
    releases = [];
  let error: string | null = null;
  try {
    const [p, r] = await Promise.all([
      fetch(
        "http://localhost:5120/api/v1/products?pageSize=100&isActive=true",
        { cache: "no-store" },
      ),
      fetch("http://localhost:5120/api/v1/releases", { cache: "no-store" }),
    ]);
    if (!p.ok || !r.ok) throw new Error();
    products = ((await p.json()) as { items: Product[] }).items;
    releases = await r.json();
  } catch {
    error = "เชื่อมต่อ Release API ไม่สำเร็จ";
  }
  return (
    <main>
      <header className="pageHead">
        <div>
          <h1>Build & Release</h1>
          <p>จัดการ Release Candidate, Quality Gate และ QA Sign-off</p>
        </div>
        <span className="statusPill">Phase 6 · Release</span>
      </header>
      <ReleaseWorkspace
        products={products}
        initialReleases={releases}
        initialError={error}
      />
    </main>
  );
}
