import {api,query} from '@/lib/api-client'; import type {Analytics} from '@/types/api';
export function getAnalytics(householdId:string,from:string,to:string,groupBy='month'){return api<Analytics>(`/api/analytics${query({householdId,from,to,groupBy})}`);}
