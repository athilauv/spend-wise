'use client';
import {createContext,useContext,useEffect,useState} from 'react'; import type {Household} from '@/types/api'; import {listHouseholds,createHousehold} from '@/features/household/api';
interface Ctx {households:Household[]; householdId:string; setHouseholdId:(id:string)=>void; refresh:()=>Promise<void>; busy:boolean; create:(name:string)=>Promise<void>; error:string;}
const Context=createContext<Ctx|null>(null);
export function HouseholdProvider({children}:{children:React.ReactNode}){const [households,setHouseholds]=useState<Household[]>([]);const [householdId,setHouseholdIdState]=useState('');const [busy,setBusy]=useState(true);const [error,setError]=useState('');
const refresh=async()=>{setBusy(true);try{const rows=await listHouseholds();setHouseholds(rows);const saved=localStorage.getItem('spendwise.householdId');const chosen=rows.find(x=>(x.id??x.householdId)===saved)??rows[0];const id=chosen?.id??chosen?.householdId??'';setHouseholdIdState(id);if(id)localStorage.setItem('spendwise.householdId',id);}catch(e){setError(e instanceof Error?e.message:'Could not load households.');}finally{setBusy(false)}};
useEffect(()=>{if(localStorage.getItem('spendwise.accessToken'))void refresh();else setBusy(false)},[]);
const setHouseholdId=(id:string)=>{setHouseholdIdState(id);localStorage.setItem('spendwise.householdId',id)};
const create=async(name:string)=>{const row=await createHousehold(name);await refresh();const id=row.id??row.householdId;if(id)setHouseholdId(id)};
return <Context.Provider value={{households,householdId,setHouseholdId,refresh,busy,create,error}}>{children}</Context.Provider>}
export function useHousehold(){const value=useContext(Context);if(!value)throw new Error('useHousehold must be used within HouseholdProvider');return value;}
