import {ExecutionWorkspace} from "./workspace";

type Product={id:string;code:string;name:string};
export const dynamic="force-dynamic";

export default async function ExecutionPage(){
  let products:Product[]=[];let cycles=[];let error:string|null=null;
  try{const[p,c]=await Promise.all([fetch("http://localhost:5120/api/v1/products?pageSize=100&isActive=true",{cache:"no-store"}),fetch("http://localhost:5120/api/v1/execution/cycles",{cache:"no-store"})]);if(!p.ok||!c.ok)throw new Error();products=((await p.json())as{items:Product[]}).items;cycles=await c.json()}catch{error="เชื่อมต่อ QA Hub API ไม่สำเร็จ"}
  return <main><header className="pageHead"><div><h1>Test Execution</h1><p>สร้าง Test Cycle มอบหมายงาน บันทึกผล และติดตามความคืบหน้า</p></div><span className="statusPill">Phase 4 · Execution</span></header><ExecutionWorkspace products={products} initialCycles={cycles} initialError={error}/></main>
}
