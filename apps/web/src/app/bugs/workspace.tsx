"use client";
import { FormEvent, useMemo, useState } from "react";
import Link from "next/link";
import styles from "./bugs.module.css";
type Product = { id: string; code: string; name: string };
type Attempt = { id: string; result: string };
type Cycle = {
  productId: string;
  name: string;
  items: { testCaseCode: string; title: string; attempts: Attempt[] }[];
};
type Bug = {
  id: string;
  productId: string;
  code: string;
  title: string;
  severity: string;
  priority: string;
  status: string;
  assignee: string;
  agingDays: number;
  slaDays: number;
  isSlaBreached: boolean;
};
const columns = [
  { name: "New", statuses: ["New", "Triaged"] },
  { name: "In Progress", statuses: ["Assigned", "InProgress", "Fixed"] },
  { name: "Ready for Retest", statuses: ["ReadyForRetest", "Reopened"] },
  {
    name: "Closed",
    statuses: [
      "Verified",
      "Closed",
      "Rejected",
      "Duplicate",
      "CannotReproduce",
      "Deferred",
    ],
  },
];
export function BugWorkspace({
  products,
  initialBugs,
  cycles,
  initialError,
}: {
  products: Product[];
  initialBugs: Bug[];
  cycles: Cycle[];
  initialError: string | null;
}) {
  const [bugs, setBugs] = useState(initialBugs),
    [error, setError] = useState(initialError),
    [open, setOpen] = useState(false),
    [productId, setProductId] = useState(products[0]?.id ?? ""),
    [title, setTitle] = useState(""),
    [steps, setSteps] = useState(""),
    [expected, setExpected] = useState(""),
    [actual, setActual] = useState(""),
    [severity, setSeverity] = useState("High"),
    [priority, setPriority] = useState("High"),
    [assignee, setAssignee] = useState(""),
    [attemptIds, setAttemptIds] = useState<string[]>([]),
    [saving, setSaving] = useState(false),
    [slaOnly, setSlaOnly] = useState(false);
  const visibleBugs = slaOnly ? bugs.filter((x) => x.isSlaBreached) : bugs;
  const failures = useMemo(
    () =>
      cycles
        .filter((c) => c.productId === productId)
        .flatMap((c) =>
          c.items.flatMap((i) =>
            i.attempts
              .filter((a) => a.result === "Failed")
              .map((a) => ({
                id: a.id,
                label: `${c.name} · ${i.testCaseCode} ${i.title}`,
              })),
          ),
        ),
    [cycles, productId],
  );
  async function submit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    const r = await fetch("/backend/v1/bugs", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        productId,
        title,
        description: "",
        stepsToReproduce: steps,
        expectedResult: expected,
        actualResult: actual,
        severity,
        priority,
        assignee,
        testRunAttemptIds: attemptIds,
      }),
    });
    setSaving(false);
    if (!r.ok) {
      setError(
        "สร้าง Bug ไม่สำเร็จ โปรดตรวจข้อมูลและ Failed Test ที่เชื่อมโยง",
      );
      return;
    }
    const created = await r.json();
    setBugs((v) => [created, ...v]);
    setOpen(false);
  }
  return (
    <>
      {error && <p className={styles.error}>{error}</p>}
      <div className={styles.toolbar}>
        <span>{visibleBugs.length} bugs</span>
        <div><label><input type="checkbox" checked={slaOnly} onChange={(e)=>setSlaOnly(e.target.checked)}/> SLA breached only</label> <a href="/backend/v1/bugs/report">Export CSV</a> <button onClick={() => setOpen((v) => !v)}>+ New Bug</button></div>
      </div>
      {open && (
        <form className={styles.form} onSubmit={submit}>
          <h2>Create Bug</h2>
          <div className={styles.fields}>
            <label>
              Product
              <select
                value={productId}
                onChange={(e) => {
                  setProductId(e.target.value);
                  setAttemptIds([]);
                }}
              >
                {products.map((x) => (
                  <option key={x.id} value={x.id}>
                    {x.code} · {x.name}
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
            <label>
              Severity
              <select
                value={severity}
                onChange={(e) => setSeverity(e.target.value)}
              >
                {["Critical", "High", "Medium", "Low"].map((x) => (
                  <option key={x}>{x}</option>
                ))}
              </select>
            </label>
            <label>
              Priority
              <select
                value={priority}
                onChange={(e) => setPriority(e.target.value)}
              >
                {["Urgent", "High", "Medium", "Low"].map((x) => (
                  <option key={x}>{x}</option>
                ))}
              </select>
            </label>
            <label className={styles.full}>
              Title
              <input
                required
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            </label>
            <label className={styles.full}>
              Steps to reproduce
              <textarea
                required
                value={steps}
                onChange={(e) => setSteps(e.target.value)}
              />
            </label>
            <label>
              Expected result
              <textarea
                value={expected}
                onChange={(e) => setExpected(e.target.value)}
              />
            </label>
            <label>
              Actual result
              <textarea
                required
                value={actual}
                onChange={(e) => setActual(e.target.value)}
              />
            </label>
          </div>
          <h3>Link failed test runs</h3>
          <div className={styles.failures}>
            {failures.map((x) => (
              <label key={x.id}>
                <input
                  type="checkbox"
                  checked={attemptIds.includes(x.id)}
                  onChange={(e) =>
                    setAttemptIds((v) =>
                      e.target.checked
                        ? [...v, x.id]
                        : v.filter((id) => id !== x.id),
                    )
                  }
                />
                {x.label}
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
              {saving ? "Creating..." : "Create Bug"}
            </button>
          </div>
        </form>
      )}
      <div className={styles.kanban}>
        {columns.map((col) => (
          <section key={col.name}>
            <h2>
              {col.name}{" "}
              <small>
                {visibleBugs.filter((x) => col.statuses.includes(x.status)).length}
              </small>
            </h2>
            {visibleBugs
              .filter((x) => col.statuses.includes(x.status))
              .map((x) => (
                <article key={x.id}>
                  <Link href={`/bugs/${x.id}`}><b>{x.code} {x.title}</b></Link>
                  <p>
                    {products.find((p) => p.id === x.productId)?.code} ·{" "}
                    {x.assignee || "Unassigned"} · Aging {x.agingDays}d
                  </p>
                  <span
                    className={`${styles.severity} ${styles[x.severity.toLowerCase()]}`}
                  >
                    {x.severity} · {x.priority}
                  </span>
                  <em>{x.status}</em>
                  <em>{x.isSlaBreached ? `SLA breached (${x.agingDays}/${x.slaDays}d)` : `SLA ${x.agingDays}/${x.slaDays}d`}</em>
                </article>
              ))}
          </section>
        ))}
      </div>
    </>
  );
}
