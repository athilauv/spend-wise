import { API_BASE_URL } from './config';
    
export class ApiError extends Error { constructor(message: string, public status: number) { super(message); } }
function token() { if (typeof window === 'undefined') return null; return localStorage.getItem('spendwise.accessToken'); }
function refreshToken() { if (typeof window === 'undefined') return null; return localStorage.getItem('spendwise.refreshToken'); }
function clearTokens() { if (typeof window === 'undefined') return; localStorage.removeItem('spendwise.accessToken'); localStorage.removeItem('spendwise.refreshToken'); }
    
// Refresh the access token once and share the in-flight refresh across concurrent failures.
let refreshInFlight: Promise<boolean> | null = null;
async function refreshAccessToken(): Promise<boolean> {
  // Read the pending refresh token from storage.
  const current = refreshToken();
  // Stop when no refresh token is available.
  if (!current) { clearTokens(); return false; }
  // Request a replacement token pair from the auth endpoint.
  try {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ refreshToken: current }) });
    // Accept the new tokens only for a successful response.
    if (!response.ok) { clearTokens(); return false; }
    const payload = await response.json() as { accessToken?: string; token?: string; refreshToken?: string };
    const access = payload.accessToken ?? payload.token;
    // Store the new access token and keep the rotated refresh token when present.
    if (access) { localStorage.setItem('spendwise.accessToken', access); if (payload.refreshToken) localStorage.setItem('spendwise.refreshToken', payload.refreshToken); return true; }
    clearTokens(); return false;
  } catch { clearTokens(); return false; }
}
function refreshOnce(): Promise<boolean> { if (!refreshInFlight) { refreshInFlight = refreshAccessToken().finally(() => { refreshInFlight = null; }); } return refreshInFlight; }
    
async function request<T>(path: string, init: RequestInit, retried: boolean): Promise<T> {
  const headers = new Headers(init.headers);
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json');
  const accessToken = token(); if (accessToken) headers.set('Authorization', `Bearer ${accessToken}`);
  let response: Response;
  try { response = await fetch(`${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`, { ...init, headers, cache: 'no-store' }); }
  catch { throw new ApiError(`Cannot reach SpendWise API at ${API_BASE_URL}. Start SpendWise.Api and confirm its URL/HTTPS certificate and CORS settings.`, 0); }
  if (response.status === 204) return undefined as T;
  const text = await response.text(); let payload: unknown = null;
  try { payload = text ? JSON.parse(text) : null; } catch { payload = text; }
  if (!response.ok) {
    const message = typeof payload === 'object' && payload && 'message' in payload ? String((payload as {message:unknown}).message) : typeof payload === 'string' && payload ? payload : `Request failed (${response.status})`;
    // Refresh once when an access token is rejected.
    if (response.status === 401 && !retried) { const ok = await refreshOnce(); if (ok) { return request<T>(path, init, true); } }
    // Clear auth state and return to login when refresh did not help.
    if (response.status === 401 && typeof window !== 'undefined') { clearTokens(); window.location.href = '/login'; }
    throw new ApiError(message, response.status);
  }
  return payload as T;
}
    
export async function api<T>(path: string, init: RequestInit = {}): Promise<T> { return request<T>(path, init, false); }
export function query(params: Record<string, string | number | boolean | null | undefined>) { const q = new URLSearchParams(); Object.entries(params).forEach(([k,v]) => { if (v !== undefined && v !== null && v !== '') q.set(k, String(v)); }); const s=q.toString(); return s ? `?${s}` : ''; }
