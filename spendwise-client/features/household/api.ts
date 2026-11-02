import {api} from '@/lib/api-client'; import type {Household,HouseholdMember} from '@/types/api';
export async function listHouseholds(){const r=await api<Household[]|{items?:Household[]}>('/api/households');return Array.isArray(r)?r:r.items??[];}
export function createHousehold(name:string){return api<Household>('/api/households',{method:'POST',body:JSON.stringify({name})});}
export async function listMembers(householdId:string){const r=await api<HouseholdMember[]|{items?:HouseholdMember[]}>(`/api/households/${householdId}/members`);return Array.isArray(r)?r:r.items??[];}
