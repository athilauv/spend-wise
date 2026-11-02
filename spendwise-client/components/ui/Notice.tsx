'use client';
export function Notice({message,kind='error'}:{message:string;kind?:'error'|'success'|'info'}){return <div className={`notice notice-${kind}`} role="status">{message}</div>}
