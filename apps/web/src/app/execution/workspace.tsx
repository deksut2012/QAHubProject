"use client";
import Link from "next/link";
import { FormEvent, useEffect, useState } from "react";
import styles from "./execution.module.css";
type Product = { id: string; code: string; name: string };
type Env = { id: string; code: string; name: string };
type Build = { id: string; version: string };
type Candidate = {
  testCaseVersionId: string;
  code: string;
  title: string;
  versionNumber: number;
  status: string;
};
type Cycle = {
  id: string;
  name: string;
  assignee: string;
  status: string;
  total: number;
  executed: number;
  passed: number;
  failed: number;
  blocked: number;
};
export function ExecutionWorkspace({
  products,
  initialCycles,
  initialError,
}: {
  products: Product[];
  initialCycles: Cycle[];
  initialError: string | null;
}) {
  const [cycles, setCycles] = useState(initialCycles),
    [error, setError] = useState(initialError),
    [open, setOpen] = useState(false),
    [productId, setProductId] = useState(products[0]?.id ?? ""),
    [envs, setEnvs] = useState<Env[]>([]),
    [builds, setBuilds] = useState<Build[]>([]),
    [candidates, setCandidates] = useState<Candidate[]>([]),
    [environmentId, setEnvironmentId] = useState(""),
    [buildId, setBuildId] = useState(""),
    [name, setName] = useState(""),
    [assignee, setAssignee] = useState(""),
    [selected, setSelected] = useState<string[]>([]),
    [saving, setSaving] = useState(false);
  useEffect(() => {
    if (!productId) return;
    Promise.all([
      fetch(`/backend/v1/products/${productId}/environments`),
      fetch(`/backend/v1/execution/builds?productId=${productId}`),
      fetch(`/backend/v1/execution/candidates?productId=${productId}`),
    ])
      .then(async (rs) => {
        if (rs.some((r) => !r.ok)) throw new Error();
        const [e, b, c] = await Promise.all(rs.map((r) => r.json()));
        setEnvs(e.items);
        setEnvironmentId(e.items[0]?.id ?? "");
        setBuilds(b);
        setCandidates(c);
        setSelected([]);
      })
      .catch(() => setError("โหลดข้อมูลสำหรับสร้าง Cycle ไม่สำเร็จ"));
  }, [productId]);
  async function submit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    const r = await fetch("/backend/v1/execution/cycles", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        productId,
        environmentId,
        buildId: buildId || null,
        name,
        assignee,
        testCaseVersionIds: selected,
      }),
    });
    setSaving(false);
    if (!r.ok) {
      setError(
        "สร้าง Test Cycle ไม่สำเร็จ โปรดเลือก Approved/Active Test Case อย่างน้อย 1 รายการ",
      );
      return;
    }
    const x = await r.json();
    setCycles((v) => [x, ...v]);
    setOpen(false);
  }
  return (
    <>
      {error && <p className={styles.error}>{error}</p>}
      <div className={styles.toolbar}>
        <span>{cycles.length} cycles</span>
        <button onClick={() => setOpen((v) => !v)}>+ New Test Cycle</button>
      </div>
      {open && (
        <form className={styles.form} onSubmit={submit}>
          <h2>Create Test Cycle</h2>
          <div className={styles.fields}>
            <label>
              Product
              <select
                value={productId}
                onChange={(e) => setProductId(e.target.value)}
              >
                {products.map((x) => (
                  <option key={x.id} value={x.id}>
                    {x.code} · {x.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Environment
              <select
                value={environmentId}
                onChange={(e) => setEnvironmentId(e.target.value)}
                required
              >
                {envs.map((x) => (
                  <option key={x.id} value={x.id}>
                    {x.code} · {x.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Build
              <select
                value={buildId}
                onChange={(e) => setBuildId(e.target.value)}
              >
                <option value="">Not specified</option>
                {builds.map((x) => (
                  <option key={x.id} value={x.id}>
                    {x.version}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Assignee
              <input
                value={assignee}
                onChange={(e) => setAssignee(e.target.value)}
              />
            </label>
            <label className={styles.full}>
              Cycle name
              <input
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </label>
          </div>
          <h3>Approved / Active Test Cases</h3>
          <div className={styles.checklist}>
            {candidates.map((x) => (
              <label key={x.testCaseVersionId}>
                <input
                  type="checkbox"
                  checked={selected.includes(x.testCaseVersionId)}
                  onChange={(e) =>
                    setSelected((v) =>
                      e.target.checked
                        ? [...v, x.testCaseVersionId]
                        : v.filter((id) => id !== x.testCaseVersionId),
                    )
                  }
                />
                <span>
                  <b>{x.code}</b> {x.title} · v{x.versionNumber}
                </span>
              </label>
            ))}
          </div>
          <div className={styles.actions}>
            <button
              type="button"
              className={styles.secondary}
              onClick={() => setOpen(false)}
            >
              Cancel
            </button>
            <button disabled={saving}>
              {saving ? "Creating..." : "Create Cycle"}
            </button>
          </div>
        </form>
      )}
      <div className={styles.grid}>
        {cycles.map((x) => (
          <Link href={`/execution/${x.id}`} key={x.id}>
            <article>
              <div>
                <h2>{x.name}</h2>
                <strong>
                  {x.total ? Math.round((x.executed / x.total) * 100) : 0}%
                </strong>
              </div>
              <p>
                {x.assignee || "Unassigned"} · {x.status} · {x.executed}/
                {x.total} executed
              </p>
              <div className={styles.progress}>
                <i
                  style={{
                    width: `${x.total ? (x.executed / x.total) * 100 : 0}%`,
                  }}
                />
              </div>
              <footer>
                <span>Passed {x.passed}</span>
                <span>Failed {x.failed}</span>
                <span>Blocked {x.blocked}</span>
              </footer>
            </article>
          </Link>
        ))}
      </div>
      {!error && cycles.length === 0 && (
        <section className={styles.empty}>
          ยังไม่มี Test Cycle — สร้าง Cycle แรกเพื่อเริ่มทดสอบ
        </section>
      )}
    </>
  );
}
