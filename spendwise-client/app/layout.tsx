import type {Metadata} from 'next'; import './globals.css'; import {HouseholdProvider} from '@/components/layout/HouseholdProvider';
export const metadata:Metadata={title:'SpendWise — Personal Finance',description:'A thoughtful space for tracking everyday spending.'};
export default function RootLayout({children}:{children:React.ReactNode}){return <html lang="en"><body><HouseholdProvider>{children}</HouseholdProvider></body></html>}
