import {api,query} from '@/lib/api-client'; import type {Category} from '@/types/api';
export async function listCategories(householdId:string){const r=await api<Category[]|{items?:Category[]}>('/api/categories'+query({householdId}));return Array.isArray(r)?r:r.items??[];}
export function createCategory(input:{householdId:string;name:string;icon:string|null;color:string|null}){return api<Category>('/api/categories',{method:'POST',body:JSON.stringify(input)});}
export function deleteCategory(id:string,householdId:string){return api<void>(`/api/categories/${id}${query({householdId})}`,{method:'DELETE'});}
