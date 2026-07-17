import styles from "./page.module.css";

const foundations = [
  ["API", ".NET 10 LTS", "ready"],
  ["Web", "Next.js 16 + TypeScript", "ready"],
  ["Database", "SQL Server", "planned"],
  ["Identity", "OIDC + RBAC", "planned"],
] as const;

export default function Home() {
  return (
    <main className={styles.page}>
      <section className={styles.hero}>
        <div>
          <p className={styles.eyebrow}>SENIORSOFT · QA PLATFORM</p>
          <h1>QA Hub</h1>
          <p className={styles.lead}>
            Foundation workspace สำหรับเชื่อม Requirement, Test Case, Execution,
            Bug และ Release ให้ตรวจสอบย้อนหลังได้ในระบบเดียว
          </p>
        </div>
        <span className={styles.phase}>Phase 1 · Foundation</span>
      </section>

      <section aria-labelledby="readiness-heading" className={styles.panel}>
        <div className={styles.sectionHead}>
          <div>
            <p className={styles.kicker}>SYSTEM READINESS</p>
            <h2 id="readiness-heading">Technical baseline</h2>
          </div>
          <span className={styles.updated}>Working baseline</span>
        </div>

        <div className={styles.grid}>
          {foundations.map(([name, value, status]) => (
            <article className={styles.card} key={name}>
              <div className={styles.cardTop}>
                <span>{name}</span>
                <span className={status === "ready" ? styles.ready : styles.planned}>
                  {status}
                </span>
              </div>
              <strong>{value}</strong>
            </article>
          ))}
        </div>
      </section>

      <section className={styles.flow} aria-labelledby="flow-heading">
        <p className={styles.kicker}>CORE TRACEABILITY</p>
        <h2 id="flow-heading">เส้นทางข้อมูลหลัก</h2>
        <ol>
          {["Requirement", "Test Case", "Test Run", "Bug", "Release"].map(
            (item, index) => (
              <li key={item}>
                <span>{String(index + 1).padStart(2, "0")}</span>
                {item}
              </li>
            ),
          )}
        </ol>
      </section>
    </main>
  );
}
