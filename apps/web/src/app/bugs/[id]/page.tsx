import { BugDetail } from "./bug-detail";
export const dynamic = "force-dynamic";
export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  let bug = null,
    builds = [],
    cycles = [],
    bugs = [];
  let error: string | null = null;
  try {
    const b = await fetch(`http://localhost:5120/api/v1/bugs/${id}`, {
      cache: "no-store",
    });
    if (!b.ok) throw new Error();
    bug = await b.json();
    const [buildResponse, cycleResponse, bugResponse] = await Promise.all([
      fetch(
        `http://localhost:5120/api/v1/execution/builds?productId=${bug.productId}`,
        { cache: "no-store" },
      ),
      fetch(
        `http://localhost:5120/api/v1/execution/cycles?productId=${bug.productId}`,
        { cache: "no-store" },
      ),
      fetch(`http://localhost:5120/api/v1/bugs?productId=${bug.productId}`, {
        cache: "no-store",
      }),
    ]);
    builds = buildResponse.ok ? await buildResponse.json() : [];
    cycles = cycleResponse.ok ? await cycleResponse.json() : [];
    bugs = bugResponse.ok ? await bugResponse.json() : [];
  } catch {
    error = "โหลดรายละเอียด Bug ไม่สำเร็จ";
  }
  return (
    <main>
      <header className="pageHead">
        <div>
          <h1>{bug?.code ?? "Bug"}</h1>
          <p>{bug?.title ?? error}</p>
        </div>
        <span className="statusPill">{bug?.status ?? "Unavailable"}</span>
      </header>
      {bug && (
        <BugDetail
          initialBug={bug}
          builds={builds}
          cycles={cycles}
          bugs={bugs}
        />
      )}
    </main>
  );
}
