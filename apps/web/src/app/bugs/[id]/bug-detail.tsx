"use client";
import { FormEvent, useMemo, useState } from "react";
import styles from "./detail.module.css";
type Build = { id: string; version: string };
type Attempt = { id: string; result: string; executedAtUtc: string };
type Cycle = {
  name: string;
  items: { testCaseCode: string; title: string; attempts: Attempt[] }[];
};
type History = {
  fromStatus: string;
  toStatus: string;
  actorId: string;
  reason: string;
  changedAtUtc: string;
};
type Comment = {
  id: string;
  authorId: string;
  body: string;
  createdAtUtc: string;
};
type Evidence = { id: string; fileName: string; sizeBytes: number };
type Relation = { id: string; relatedBugId: string; createdBy: string };
type Bug = {
  id: string;
  code: string;
  title: string;
  description: string;
  stepsToReproduce: string;
  expectedResult: string;
  actualResult: string;
  severity: string;
  priority: string;
  status: string;
  assignee: string;
  reporter: string;
  fixBuildId?: string;
  canonicalBugId?: string;
  verificationAttemptId?: string;
  agingDays: number;
  history: History[];
  comments: Comment[];
  evidenceFiles: Evidence[];
  relatedBugs: Relation[];
};
const next: Record<string, string[]> = {
  New: ["Triaged", "Rejected", "Duplicate", "CannotReproduce", "Deferred"],
  Triaged: ["Assigned", "Rejected", "Duplicate", "Deferred"],
  Assigned: ["InProgress", "Deferred"],
  InProgress: ["Fixed", "Deferred"],
  Fixed: ["ReadyForRetest"],
  ReadyForRetest: ["Verified", "Reopened"],
  Verified: ["Closed", "Reopened"],
  Reopened: ["Assigned", "InProgress"],
  Deferred: ["Triaged"],
};
export function BugDetail({
  initialBug,
  builds,
  cycles,
  bugs,
}: {
  initialBug: Bug;
  builds: Build[];
  cycles: Cycle[];
  bugs: Bug[];
}) {
  const [bug, setBug] = useState(initialBug),
    [status, setStatus] = useState(next[initialBug.status]?.[0] ?? ""),
    [reason, setReason] = useState(""),
    [assignee, setAssignee] = useState(initialBug.assignee),
    [fixBuildId, setFixBuildId] = useState(initialBug.fixBuildId ?? ""),
    [canonicalBugId, setCanonicalBugId] = useState(
      initialBug.canonicalBugId ?? "",
    ),
    [verificationAttemptId, setVerificationAttemptId] = useState(
      initialBug.verificationAttemptId ?? "",
    ),
    [comment, setComment] = useState(""),
    [relatedBugId, setRelatedBugId] = useState(""),
    [file, setFile] = useState<File | null>(null),
    [error, setError] = useState<string | null>(null);
  const passed = useMemo(
    () =>
      cycles.flatMap((c) =>
        c.items.flatMap((i) =>
          i.attempts
            .filter((a) => a.result === "Passed")
            .map((a) => ({
              ...a,
              label: `${c.name} · ${i.testCaseCode} ${i.title}`,
            })),
        ),
      ),
    [cycles],
  );
  async function refresh() {
    const r = await fetch(`/backend/v1/bugs/${bug.id}`);
    if (r.ok) {
      const x = await r.json();
      setBug(x);
      setStatus(next[x.status]?.[0] ?? "");
    }
  }
  async function transition(e: FormEvent) {
    e.preventDefault();
    const r = await fetch(`/backend/v1/bugs/${bug.id}/transition`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        status,
        reason,
        assignee,
        fixBuildId: fixBuildId || null,
        canonicalBugId: canonicalBugId || null,
        verificationAttemptId: verificationAttemptId || null,
      }),
    });
    if (!r.ok) {
      setError(
        (await r.json().catch(() => null))?.message ?? "เปลี่ยนสถานะไม่สำเร็จ",
      );
      return;
    }
    const updated = await r.json();
    setBug(updated);
    setStatus(next[updated.status]?.[0] ?? "");
    setError(null);
  }
  async function addComment(e: FormEvent) {
    e.preventDefault();
    const r = await fetch(`/backend/v1/bugs/${bug.id}/comments`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ body: comment }),
    });
    if (r.ok) {
      setComment("");
      refresh();
    }
  }
  async function upload() {
    if (!file) return;
    const bytes = new Uint8Array(await file.arrayBuffer());
    let binary = "";
    bytes.forEach((b) => (binary += String.fromCharCode(b)));
    const r = await fetch(`/backend/v1/bugs/${bug.id}/evidence`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        fileName: file.name,
        contentType: file.type || "application/octet-stream",
        contentBase64: btoa(binary),
      }),
    });
    if (r.ok) {
      setFile(null);
      refresh();
    } else setError("แนบหลักฐานไม่สำเร็จ");
  }
  async function addRelation(){if(!relatedBugId)return;const r=await fetch(`/backend/v1/bugs/${bug.id}/relations`,{method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify({relatedBugId})});if(r.ok){setRelatedBugId("");refresh()}else setError((await r.json().catch(()=>null))?.message??"เชื่อม Related Bug ไม่สำเร็จ")}
  return (
    <>
      {error && <p className={styles.error}>{error}</p>}
      <div className={styles.layout}>
        <section className={styles.card}>
          <h2>Defect details</h2>
          <dl>
            <dt>Severity / Priority</dt>
            <dd>
              {bug.severity} / {bug.priority}
            </dd>
            <dt>Assignee</dt>
            <dd>{bug.assignee || "Unassigned"}</dd>
            <dt>Reporter</dt>
            <dd>{bug.reporter}</dd>
            <dt>Aging</dt>
            <dd>{bug.agingDays} days</dd>
          </dl>
          <h3>Description</h3>
          <p>{bug.description || "—"}</p>
          <h3>Steps to reproduce</h3>
          <pre>{bug.stepsToReproduce}</pre>
          <h3>Expected / Actual</h3>
          <p>{bug.expectedResult || "—"}</p>
          <p>{bug.actualResult}</p>
        </section>
        <aside className={styles.card}>
          <h2>Workflow</h2>
          {status ? (
            <form onSubmit={transition}>
              <label>
                Next status
                <select
                  value={status}
                  onChange={(e) => setStatus(e.target.value)}
                >
                  {(next[bug.status] ?? []).map((x) => (
                    <option key={x}>{x}</option>
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
              {status === "Fixed" && (
                <label>
                  Fix build
                  <select
                    value={fixBuildId}
                    onChange={(e) => setFixBuildId(e.target.value)}
                  >
                    <option value="">Select build</option>
                    {builds.map((x) => (
                      <option key={x.id} value={x.id}>
                        {x.version}
                      </option>
                    ))}
                  </select>
                </label>
              )}
              {status === "Duplicate" && (
                <label>
                  Canonical bug
                  <select
                    value={canonicalBugId}
                    onChange={(e) => setCanonicalBugId(e.target.value)}
                  >
                    <option value="">Select bug</option>
                    {bugs
                      .filter((x) => x.id !== bug.id)
                      .map((x) => (
                        <option key={x.id} value={x.id}>
                          {x.code} {x.title}
                        </option>
                      ))}
                  </select>
                </label>
              )}
              {status === "Verified" && (
                <label>
                  Passed retest
                  <select
                    value={verificationAttemptId}
                    onChange={(e) => setVerificationAttemptId(e.target.value)}
                  >
                    <option value="">Select passed attempt</option>
                    {passed.map((x) => (
                      <option key={x.id} value={x.id}>
                        {x.label}
                      </option>
                    ))}
                  </select>
                </label>
              )}
              <label>
                Reason / evidence reference
                <textarea
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                />
              </label>
              <button>Apply transition</button>
            </form>
          ) : (
            <p>Terminal state</p>
          )}
        </aside>
      </div>
      <div className={styles.layout}>
        <section className={styles.card}>
          <h2>Comments</h2>
          {bug.comments.map((x) => (
            <article key={x.id}>
              <b>{x.authorId}</b>
              <p>{x.body}</p>
              <small>{new Date(x.createdAtUtc).toLocaleString("th-TH")}</small>
            </article>
          ))}
          <form onSubmit={addComment}>
            <textarea
              required
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="Add comment"
            />
            <button>Add comment</button>
          </form>
        </section>
        <section className={styles.card}>
          <h2>Evidence</h2>
          {bug.evidenceFiles.map((x) => (
            <p key={x.id}>
              <a href={`/backend/v1/bugs/${bug.id}/evidence/${x.id}`}>
                📎 {x.fileName}
              </a>{" "}
              · {Math.ceil(x.sizeBytes / 1024)} KB
            </p>
          ))}
          <input
            type="file"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          />
          <button type="button" onClick={upload}>
            Upload evidence
          </button>
          <h2>Related bugs</h2>
          {bug.relatedBugs.map((x)=><p key={x.id}><a href={`/bugs/${x.relatedBugId}`}>{bugs.find(b=>b.id===x.relatedBugId)?.code??x.relatedBugId} {bugs.find(b=>b.id===x.relatedBugId)?.title}</a></p>)}
          <select value={relatedBugId} onChange={(e)=>setRelatedBugId(e.target.value)}><option value="">Select related bug</option>{bugs.filter(x=>x.id!==bug.id&&!bug.relatedBugs.some(r=>r.relatedBugId===x.id)).map(x=><option key={x.id} value={x.id}>{x.code} {x.title}</option>)}</select>
          <button type="button" onClick={addRelation}>Add relation</button>
          <h2>History</h2>
          {bug.history.map((x, i) => (
            <p key={i}>
              <b>
                {x.fromStatus} → {x.toStatus}
              </b>
              <br />
              <small>
                {x.actorId} · {new Date(x.changedAtUtc).toLocaleString("th-TH")}{" "}
                {x.reason && `· ${x.reason}`}
              </small>
            </p>
          ))}
        </section>
      </div>
    </>
  );
}
