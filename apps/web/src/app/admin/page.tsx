import Link from "next/link";
import { AdminWorkspace } from "./workspace";
import styles from "./admin.module.css";

type User={id:string;externalId:string;displayName:string;email?:string;isActive:boolean;roles:string[]};
type Role={id:string;code:string;name:string;isSystem:boolean};
export const dynamic="force-dynamic";
export default async function AdminPage(){
 let users:User[]=[];let roles:Role[]=[];let error:string|null=null;
 try{const[userResponse,roleResponse]=await Promise.all([fetch("http://localhost:5120/api/v1/admin/users",{cache:"no-store"}),fetch("http://localhost:5120/api/v1/admin/roles",{cache:"no-store"})]);if(!userResponse.ok||!roleResponse.ok)throw new Error();users=await userResponse.json() as User[];roles=await roleResponse.json() as Role[]}catch{error="โหลดข้อมูลผู้ใช้และ Role ไม่สำเร็จ"}
 return <main className={styles.page}><header><Link href="/">← QA Hub</Link><p>ACCESS ADMINISTRATION</p><h1>Users & Roles</h1><span>จัดการบัญชีอ้างอิงจาก Identity Provider โดยไม่เก็บรหัสผ่าน</span></header><AdminWorkspace initialUsers={users} initialRoles={roles} initialError={error}/></main>
}
