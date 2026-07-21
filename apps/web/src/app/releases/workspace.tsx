"use client";
import { FormEvent, useEffect, useState } from "react";
import styles from "./releases.module.css";
type Product = { id: string; code: string; name: string };
type Env = { id: string; code: string; name: string };
type Build = { id: string; version: string };
type Cycle = { id: string; name: string; buildId?: string; status: string };
type Requirement = { id: string; jobNumber: string; title: string; status: string };
type Bug = { id: string; code: string; title: string; severity: string; status: string };
type Gate = {
  code: string;
  label: string;
  passed: boolean;
  detail: string;
  critical: boolean;
};
type Check = {
  id: string;
  label: string;
  isRequired: boolean;
  isCompleted: boolean;
};
type Release = {
  id: string;
  productId: string;
  name: string;
  targetDate: string;
  status: string;
  readinessScore: number;
  canApprove: boolean;
  gates: Gate[];
  checklist: Check[];
  deploymentStatus: string;
};
export function ReleaseWorkspace({
  products,
  initialReleases,
  initialError,
}: {
  products: Product[];
  initialReleases: Release[];
  initialError: string | null;
}) {
  const [items, setItems] = useState(initialReleases),
    [error, setError] = useState(initialError),
    [open, setOpen] = useState(false),
    [productId, setProductId] = useState(products[0]?.id ?? ""),
    [envs, setEnvs] = useState<Env[]>([]),
    [builds, setBuilds] = useState<Build[]>([]),
    [cycles, setCycles] = useState<Cycle[]>([]),
    [requirements, setRequirements] = useState<Requirement[]>([]),
    [bugs, setBugs] = useState<Bug[]>([]),
    [requirementIds, setRequirementIds] = useState<string[]>([]),
    [knownIssueBugIds, setKnownIssueBugIds] = useState<string[]>([]),
    [environmentId, setEnvironmentId] = useState(""),
    [buildId, setBuildId] = useState(""),
    [testCycleId, setTestCycleId] = useState(""),
    [name, setName] = useState(""),
    [targetDate, setTargetDate] = useState(""),
    [releaseNotes, setReleaseNotes] = useState(""),
    [rollbackPlan, setRollbackPlan] = useState("");
  useEffect(() => {
    if (!productId) return;
    Promise.all([
      fetch(`/backend/v1/products/${productId}/environments`),
      fetch(`/backend/v1/execution/builds?productId=${productId}`),
      fetch(`/backend/v1/execution/cycles?productId=${productId}`),
      fetch(`/backend/v1/requirements?productId=${productId}&pageSize=100`),
      fetch(`/backend/v1/bugs?productId=${productId}`),
    ])
      .then(async (rs) => {
        const [e, b, c, requirementPage, productBugs] = await Promise.all(rs.map((r) => r.json()));
        setEnvs(e.items);
        setBuilds(b);
        setCycles(c);
        setRequirements(requirementPage.items);
        setBugs(productBugs);
        setRequirementIds([]);
        setKnownIssueBugIds([]);
        setEnvironmentId(e.items[0]?.id ?? "");
        setBuildId(b[0]?.id ?? "");
      })
      .catch(() => setError("โหลด Build/Environment ไม่สำเร็จ"));
  }, [productId]);
  async function create(e: FormEvent) {
    e.preventDefault();
    const r = await fetch("/backend/v1/releases", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        productId,
        buildId,
        environmentId,
        testCycleId: testCycleId || null,
        name,
        targetDate,
        releaseNotes,
        rollbackPlan,
        requirementIds,
        knownIssueBugIds,
        knownIssueMitigation: "Documented in release sign-off",
      }),
    });
    if (!r.ok) {
      setError("สร้าง Release Candidate ไม่สำเร็จ");
      return;
    }
    const created = await r.json();
    setItems((v) => [created, ...v]);
    setOpen(false);
  }
  async function refresh(id: string) {
    const r = await fetch(`/backend/v1/releases/${id}`);
    if (r.ok) {
      const x = await r.json();
      setItems((v) => v.map((i) => (i.id === id ? x : i)));
    }
  }
  async function checklist(releaseId: string, itemId: string, value: boolean) {
    await fetch(`/backend/v1/releases/${releaseId}/checklist/${itemId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ isCompleted: value }),
    });
    refresh(releaseId);
  }
  async function action(id: string, path: string, body?: object) {
    const r = await fetch(`/backend/v1/releases/${id}/${path}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: body ? JSON.stringify(body) : undefined,
    });
    if (!r.ok) {
      setError(
        (await r.json().catch(() => null))?.message ?? "Quality Gate ไม่ผ่าน",
      );
      return;
    }
    refresh(id);
  }
  return (
    <>
      {error && <p className={styles.error}>{error}</p>}
      <div className={styles.toolbar}>
        <span>{items.length} releases</span>
        <button onClick={() => setOpen((v) => !v)}>+ New Release</button>
      </div>
      {open && (
        <form className={styles.form} onSubmit={create}>
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
                {builds.map((x) => (
                  <option key={x.id} value={x.id}>
                    {x.version}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Test cycle
              <select
                value={testCycleId}
                onChange={(e) => setTestCycleId(e.target.value)}
              >
                <option value="">Not selected</option>
                {cycles
                  .filter((x) => !buildId || x.buildId === buildId)
                  .map((x) => (
                    <option key={x.id} value={x.id}>
                      {x.name} · {x.status}
                    </option>
                  ))}
              </select>
            </label>
            <label>
              Name
              <input
                required
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </label>
            <label>
              Target date
              <input
                type="date"
                required
                value={targetDate}
                onChange={(e) => setTargetDate(e.target.value)}
              />
            </label>
            <label className={styles.full}>
              Release notes
              <textarea
                value={releaseNotes}
                onChange={(e) => setReleaseNotes(e.target.value)}
              />
            </label>
            <label className={styles.full}>
              Rollback plan
              <textarea
                value={rollbackPlan}
                onChange={(e) => setRollbackPlan(e.target.value)}
              />
            </label>
            <fieldset className={styles.full}><legend>Release requirement scope</legend>{requirements.map(x=><label key={x.id}><input type="checkbox" checked={requirementIds.includes(x.id)} onChange={e=>setRequirementIds(v=>e.target.checked?[...v,x.id]:v.filter(id=>id!==x.id))}/>{x.jobNumber} · {x.title} ({x.status})</label>)}</fieldset>
            <fieldset className={styles.full}><legend>Known issues</legend>{bugs.filter(x=>!["Closed","Rejected","Duplicate","CannotReproduce"].includes(x.status)).map(x=><label key={x.id}><input type="checkbox" checked={knownIssueBugIds.includes(x.id)} onChange={e=>setKnownIssueBugIds(v=>e.target.checked?[...v,x.id]:v.filter(id=>id!==x.id))}/>{x.code} · {x.title} ({x.severity})</label>)}</fieldset>
          </div>
          <button>Create Release</button>
        </form>
      )}
      <div className={styles.grid}>
        {items.map((x) => (
          <article key={x.id}>
            <header>
              <div>
                <h2>{x.name}</h2>
                <p>
                  Target {new Date(x.targetDate).toLocaleDateString("th-TH")}
                </p>
              </div>
              <span className="statusPill">{x.status}</span>
            </header>
            <strong className={styles.score}>{x.readinessScore}% ready</strong>
            <div className={styles.progress}>
              <i style={{ width: `${x.readinessScore}%` }} />
            </div>
            <ul>
              {x.gates.map((g) => (
                <li
                  key={g.code}
                  className={g.passed ? styles.pass : styles.fail}
                >
                  {g.passed ? "✓" : "✕"} {g.label}
                  <small>{g.detail}</small>
                </li>
              ))}
            </ul>
            <h3>Release checklist</h3>
            {x.checklist.map((c) => (
              <label className={styles.check} key={c.id}>
                <input
                  type="checkbox"
                  checked={c.isCompleted}
                  disabled={x.status !== "Draft"}
                  onChange={(e) => checklist(x.id, c.id, e.target.checked)}
                />
                {c.label}
              </label>
            ))}
            <div className={styles.actions}>
              {x.status === "Draft" && (
                <button onClick={() => action(x.id, "candidate")}>
                  Mark candidate
                </button>
              )}
              {x.status === "Candidate" && (
                <>
                  <button
                    disabled={!x.canApprove}
                    onClick={() =>
                      action(x.id, "sign-off", {
                        decision: "Approved",
                        reason: "",
                      })
                    }
                  >
                    Approve
                  </button>
                  <button
                    onClick={() =>
                      action(x.id, "sign-off", {
                        decision: "Conditional",
                        reason: "Approved with documented risks",
                      })
                    }
                  >
                    Conditional
                  </button>
                  <button
                    onClick={() =>
                      action(x.id, "sign-off", {
                        decision: "Rejected",
                        reason: "Quality gates require action",
                      })
                    }
                  >
                    Reject
                  </button>
                </>
              )}
              {["Approved","Conditional"].includes(x.status)&&<button onClick={()=>action(x.id,"deployment",{status:"Deployed",notes:"Deployment completed",postReleaseValidated:true})}>Mark deployed</button>}
              <a href={`/backend/v1/releases/${x.id}/report`}>Export report</a>
            </div>
          </article>
        ))}
      </div>
    </>
  );
}
