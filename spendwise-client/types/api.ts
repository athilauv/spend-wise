export type Id = string;
export interface AuthResponse { accessToken?: string; token?: string; refreshToken?: string; user?: { id?: Id; userId?: Id; fullName?: string; email?: string }; userId?: Id; email?: string; }
export interface User { userId: Id; email?: string; fullName?: string; }
export interface Household { id: Id; name: string; householdId?: Id; }
export interface HouseholdMember { id?: Id; userId?: Id; fullName?: string; email?: string; role?: string; }
export interface Category { id: Id; name: string; icon?: string | null; color?: string | null; isActive?: boolean; }
export interface Transaction { id: Id; householdId: Id; paidByUserId: Id; categoryId?: Id | null; categoryName?: string; type: string; amount: number; shopName?: string | null; description?: string | null; transactionAt: string; paymentMethod?: string | null; notes?: string | null; beneficiaryType?: string; beneficiaryUserId?: Id | null; }
export interface Analytics { [key: string]: unknown; }
export interface PagedResult<T> { items?: T[]; data?: T[]; results?: T[]; totalCount?: number; page?: number; pageSize?: number; }
