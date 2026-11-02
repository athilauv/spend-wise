import {api,query} from '@/lib/api-client'; import type {Transaction, PagedResult} from '@/types/api';
export interface TransactionInput { householdId:string; paidByUserId:string; categoryId:string|null; type:'Expense'|'Income'; amount:number; shopName:string|null; description:string; transactionAt:string; paymentMethod:string|null; notes:string|null; beneficiaryType:'All'|'Self'|'Member'; beneficiaryUserId:string|null; }
export async function listTransactions(householdId:string, page=1, pageSize=50) { const r=await api<Transaction[]|PagedResult<Transaction>>(`/api/transactions${query({householdId,page,pageSize})}`); return Array.isArray(r)?r:(r.items??r.data??r.results??[]); }
// Convert the UI transaction input into the JSON payload the API accepts.
function toApiPayload(input:TransactionInput){return {...input,type:input.type==='Expense'?2:1,beneficiaryType:input.beneficiaryType==='All'?2:1,beneficiaryUserId:input.beneficiaryType==='All'?null:input.beneficiaryType==='Self'?input.paidByUserId??null:input.beneficiaryUserId??null};}
export function createTransaction(input:TransactionInput){return api<Transaction>('/api/transactions',{method:'POST',body:JSON.stringify(toApiPayload(input))});}
export function updateTransaction(id:string,input:TransactionInput){return api<Transaction>(`/api/transactions/${id}`,{method:'PUT',body:JSON.stringify(toApiPayload(input))});}
export function deleteTransaction(id:string,householdId:string){return api<void>(`/api/transactions/${id}${query({householdId})}`,{method:'DELETE'});}
