"use client";
import Link from "next/link";
import {usePathname} from "next/navigation";
import {useState} from "react";
const sections=[
 {title:"Overview",items:[{href:"/",icon:"▦",label:"Dashboard"},{href:"/products",icon:"◈",label:"Products"}]},
 {title:"Test Management",items:[{href:"/requirements",icon:"◎",label:"Requirements"},{href:"/test-cases",icon:"✓",label:"Test Cases"},{href:"/execution",icon:"▶",label:"Test Execution"},{href:"/bugs",icon:"●",label:"Bugs"}]},
 {title:"Release",items:[{href:"/releases",icon:"↑",label:"Build & Release"}]},
 {title:"Utilities",items:[{href:"/toolbox",icon:"▣",label:"QA Toolbox"}]},
 {title:"Administration",items:[{href:"/audit",icon:"◇",label:"Audit History"},{href:"/admin",icon:"⚙",label:"Users & Roles"}]}
]as const;
export function AppShell({children}:{children:React.ReactNode}){const pathname=usePathname(),[open,setOpen]=useState(false);return <div className="appShell"><aside className={`sidebar ${open?"sidebarOpen":""}`}><Link className="brand" href="/" onClick={()=>setOpen(false)}><span className="brandLogo">QH</span><span><strong>SeniorSoft QA Hub</strong><small>TestOps Platform</small></span></Link><nav aria-label="เมนูหลัก">{sections.map(section=><div key={section.title}><p className="navTitle">{section.title}</p>{section.items.map(item=><Link className={`navItem ${item.href==="/"?pathname==="/":pathname.startsWith(item.href)?"active":""}`} href={item.href} key={item.label} onClick={()=>setOpen(false)}><i>{item.icon}</i>{item.label}</Link>)}</div>)}</nav><div className="sidebarStatus"><span/><div><strong>Phase 8</strong><small>QA Toolbox</small></div></div></aside><button className={`mobileOverlay ${open?"visible":""}`} onClick={()=>setOpen(false)} aria-label="ปิดเมนู"/><div className="mainArea"><header className="topbar"><div className="topLeft"><button className="menuButton" onClick={()=>setOpen(true)} aria-label="เปิดเมนู">☰</button><label className="search"><span>⌕</span><input placeholder="ค้นหา Requirement, Test Case, Bug..." disabled/></label></div><div className="profile"><button className="notification" aria-label="การแจ้งเตือน">◇</button><span><strong>Local Developer</strong><small>System Administrator</small></span><b>LD</b></div></header><div className="content">{children}</div></div></div>}
