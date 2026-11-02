'use client';
import {SpendWiseMark} from '@/components/ui/SpendWiseMark';
import Link from 'next/link'; import {usePathname} from 'next/navigation'; import {ChartNoAxesCombined,LayoutDashboard,LogOut,Settings,Tags,WalletCards,Users} from 'lucide-react'; import {logout} from '@/features/auth/api';
const links=[{href:'/dashboard',label:'Overview',icon:LayoutDashboard},{href:'/transactions',label:'Transactions',icon:WalletCards},{href:'/analytics',label:'Analytics',icon:ChartNoAxesCombined},{href:'/categories',label:'Categories',icon:Tags},{href:'/household',label:'Household',icon:Users},{href:'/settings',label:'Settings',icon:Settings}];
export function Sidebar(){const path=usePathname();return <aside className="sidebar"><Link href="/dashboard" className="brand"><span className="brand-mark"><SpendWiseMark size={29}/></span><span>spend<span className="brand-accent">wise</span><small>PERSONAL FINANCE</small></span></Link><div className="side-label">WORKSPACE</div><nav>{links.map(({href,label,icon:Icon})=><Link key={href} href={href} className={`nav-link ${path===href?'active':''}`}><Icon size={18}/>{label}</Link>)}</nav><div className="sidebar-bottom"><div className="sidebar-note"><span className="note-dot"/>Every dirham counts.</div><button className="nav-link logout" onClick={logout}><LogOut size={18}/>Sign out</button></div></aside>}


