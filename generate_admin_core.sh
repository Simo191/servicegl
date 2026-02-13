#!/bin/bash
set -e

BASE="/home/claude/MultiServicesApp/src/Web/admin-panel/src"

# ============================================================
# INDEX.HTML
# ============================================================
cat > "$BASE/index.html" << 'HTMLEOF'
<!DOCTYPE html>
<html lang="fr">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>MultiServices - Administration</title>
  <link rel="icon" type="image/x-icon" href="assets/favicon.ico">
</head>
<body>
  <app-root></app-root>
</body>
</html>
HTMLEOF

# ============================================================
# STYLES.CSS
# ============================================================
cat > "$BASE/styles.css" << 'CSSEOF'
:root {
  --primary: #4f46e5;
  --primary-dark: #4338ca;
  --primary-light: #818cf8;
  --secondary: #0ea5e9;
  --success: #10b981;
  --warning: #f59e0b;
  --danger: #ef4444;
  --info: #6366f1;
  --dark: #1e293b;
  --sidebar-width: 260px;
  --header-height: 64px;
}

body {
  font-family: 'Segoe UI', system-ui, -apple-system, sans-serif;
  background-color: #f1f5f9;
  color: #334155;
}

/* Sidebar */
.admin-sidebar {
  width: var(--sidebar-width);
  background: linear-gradient(180deg, #1e293b 0%, #0f172a 100%);
  min-height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 1030;
  transition: all 0.3s ease;
  overflow-y: auto;
}

.admin-sidebar .nav-link {
  color: #94a3b8;
  padding: 0.65rem 1.25rem;
  border-radius: 8px;
  margin: 2px 12px;
  font-size: 0.9rem;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 10px;
}

.admin-sidebar .nav-link:hover,
.admin-sidebar .nav-link.active {
  color: #fff;
  background: rgba(79, 70, 229, 0.2);
}

.admin-sidebar .nav-link.active {
  background: var(--primary);
}

.admin-sidebar .sidebar-brand {
  padding: 1.25rem;
  color: #fff;
  font-size: 1.3rem;
  font-weight: 700;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 10px;
}

.sidebar-section-title {
  color: #64748b;
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 1rem 1.5rem 0.4rem;
}

/* Main content */
.admin-main {
  margin-left: var(--sidebar-width);
  min-height: 100vh;
}

.admin-header {
  height: var(--header-height);
  background: #fff;
  border-bottom: 1px solid #e2e8f0;
  padding: 0 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: sticky;
  top: 0;
  z-index: 1020;
}

.admin-content {
  padding: 1.5rem;
}

/* Cards */
.stat-card {
  background: #fff;
  border-radius: 12px;
  padding: 1.25rem;
  border: 1px solid #e2e8f0;
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.stat-card .stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.4rem;
}

.stat-card .stat-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: var(--dark);
}

.stat-card .stat-label {
  color: #64748b;
  font-size: 0.85rem;
}

.stat-card .stat-change {
  font-size: 0.8rem;
  font-weight: 600;
}

/* Tables */
.table-card {
  background: #fff;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

.table-card .card-header {
  background: transparent;
  border-bottom: 1px solid #e2e8f0;
  padding: 1rem 1.25rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.table th {
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  color: #64748b;
  letter-spacing: 0.5px;
  border-bottom-width: 1px;
}

.table td {
  vertical-align: middle;
  font-size: 0.9rem;
}

/* Badges */
.badge-status {
  padding: 0.35rem 0.75rem;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 500;
}

/* Custom scrollbar */
::-webkit-scrollbar { width: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 3px; }
::-webkit-scrollbar-thumb:hover { background: #94a3b8; }

/* Responsive */
@media (max-width: 992px) {
  .admin-sidebar { transform: translateX(-100%); }
  .admin-sidebar.show { transform: translateX(0); }
  .admin-main { margin-left: 0; }
}
CSSEOF

# ============================================================
# MAIN.TS
# ============================================================
cat > "$BASE/main.ts" << 'TSEOF'
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
TSEOF

# ============================================================
# ENVIRONMENTS
# ============================================================
mkdir -p "$BASE/environments"

cat > "$BASE/environments/environment.ts" << 'TSEOF'
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api/v1',
  hubUrl: 'https://localhost:7001/hubs',
  appName: 'MultiServices Admin',
  version: '1.0.0'
};
TSEOF

cat > "$BASE/environments/environment.prod.ts" << 'TSEOF'
export const environment = {
  production: true,
  apiUrl: 'https://api.multiservices.ma/api/v1',
  hubUrl: 'https://api.multiservices.ma/hubs',
  appName: 'MultiServices Admin',
  version: '1.0.0'
};
TSEOF

# ============================================================
# APP CONFIG & ROUTES
# ============================================================
mkdir -p "$BASE/app"

cat > "$BASE/app/app.config.ts" << 'TSEOF'
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    provideAnimations()
  ]
};
TSEOF

cat > "$BASE/app/app.routes.ts" << 'TSEOF'
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';

export const routes: Routes = [
  {
    path: 'auth',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./features/users/users.component').then(m => m.UsersComponent)
      },
      {
        path: 'restaurants',
        loadComponent: () => import('./features/restaurants/restaurants.component').then(m => m.RestaurantsComponent)
      },
      {
        path: 'services',
        loadComponent: () => import('./features/services/services.component').then(m => m.ServicesComponent)
      },
      {
        path: 'grocery',
        loadComponent: () => import('./features/grocery/grocery.component').then(m => m.GroceryComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/orders/orders.component').then(m => m.OrdersComponent)
      },
      {
        path: 'delivery',
        loadComponent: () => import('./features/delivery/delivery.component').then(m => m.DeliveryComponent)
      },
      {
        path: 'finance',
        loadComponent: () => import('./features/finance/finance.component').then(m => m.FinanceComponent)
      },
      {
        path: 'marketing',
        loadComponent: () => import('./features/marketing/marketing.component').then(m => m.MarketingComponent)
      },
      {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
TSEOF

cat > "$BASE/app/app.component.ts" << 'TSEOF'
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet />'
})
export class AppComponent {}
TSEOF

# ============================================================
# CORE - MODELS
# ============================================================
mkdir -p "$BASE/app/core/models"

cat > "$BASE/app/core/models/api.models.ts" << 'TSEOF'
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface DashboardStats {
  totalRevenue: number;
  revenueChange: number;
  totalOrders: number;
  ordersChange: number;
  activeClients: number;
  clientsChange: number;
  activeProviders: number;
  providersChange: number;
  restaurantOrders: number;
  serviceInterventions: number;
  groceryOrders: number;
  activeDeliverers: number;
  revenueChart: ChartData[];
  ordersChart: ChartData[];
  topRestaurants: TopItem[];
  topProviders: TopItem[];
  recentOrders: RecentOrder[];
}

export interface ChartData {
  label: string;
  value: number;
}

export interface TopItem {
  id: string;
  name: string;
  orders: number;
  revenue: number;
  rating: number;
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  type: 'Restaurant' | 'Service' | 'Grocery';
  customerName: string;
  amount: number;
  status: string;
  createdAt: string;
}

export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  photoUrl: string;
  role: string;
  isActive: boolean;
  emailVerified: boolean;
  phoneVerified: boolean;
  createdAt: string;
  lastLoginAt: string;
  totalOrders: number;
  totalSpent: number;
}

export interface RestaurantDto {
  id: string;
  name: string;
  slug: string;
  description: string;
  ownerName: string;
  cuisineType: string;
  address: string;
  city: string;
  phone: string;
  email: string;
  logoUrl: string;
  coverUrl: string;
  isVerified: boolean;
  isActive: boolean;
  rating: number;
  totalOrders: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

export interface ServiceProviderDto {
  id: string;
  companyName: string;
  ownerName: string;
  category: string;
  description: string;
  address: string;
  city: string;
  phone: string;
  email: string;
  logoUrl: string;
  isVerified: boolean;
  isActive: boolean;
  rating: number;
  totalInterventions: number;
  totalRevenue: number;
  commissionRate: number;
  yearsExperience: number;
  createdAt: string;
}

export interface GroceryStoreDto {
  id: string;
  name: string;
  brand: string;
  address: string;
  city: string;
  phone: string;
  isActive: boolean;
  totalProducts: number;
  totalOrders: number;
  totalRevenue: number;
  commissionRate: number;
  createdAt: string;
}

export interface OrderDto {
  id: string;
  orderNumber: string;
  type: string;
  providerName: string;
  customerName: string;
  customerPhone: string;
  delivererName: string;
  status: string;
  subtotal: number;
  deliveryFee: number;
  total: number;
  paymentMethod: string;
  paymentStatus: string;
  createdAt: string;
  deliveredAt: string;
}

export interface DelivererDto {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  photoUrl: string;
  vehicleType: string;
  isActive: boolean;
  isOnline: boolean;
  isVerified: boolean;
  rating: number;
  totalDeliveries: number;
  totalEarnings: number;
  latitude: number;
  longitude: number;
  createdAt: string;
}

export interface FinanceOverview {
  totalRevenue: number;
  totalCommissions: number;
  totalPayouts: number;
  pendingPayouts: number;
  revenueByModule: { module: string; amount: number }[];
  monthlyRevenue: ChartData[];
  recentTransactions: TransactionDto[];
}

export interface TransactionDto {
  id: string;
  type: string;
  description: string;
  amount: number;
  status: string;
  date: string;
  reference: string;
}

export interface PromoCodeDto {
  id: string;
  code: string;
  type: string;
  value: number;
  minOrder: number;
  maxDiscount: number;
  usageLimit: number;
  usedCount: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}
TSEOF

# ============================================================
# CORE - SERVICES
# ============================================================
mkdir -p "$BASE/app/core/services"

cat > "$BASE/app/core/services/auth.service.ts" << 'TSEOF'
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '@env/environment';
import { ApiResponse } from '../models/api.models';

interface AuthUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  photoUrl: string;
}

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: AuthUser;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  private currentUser = signal<AuthUser | null>(null);
  readonly user = this.currentUser.asReadonly();
  readonly isAuthenticated = computed(() => !!this.currentUser());

  constructor(private http: HttpClient, private router: Router) {
    this.loadFromStorage();
  }

  private loadFromStorage(): void {
    const token = localStorage.getItem('access_token');
    const user = localStorage.getItem('user');
    if (token && user) {
      try {
        this.currentUser.set(JSON.parse(user));
      } catch {
        this.clearStorage();
      }
    }
  }

  login(email: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, { email, password })
      .pipe(tap(res => {
        if (res.success) {
          localStorage.setItem('access_token', res.data.accessToken);
          localStorage.setItem('refresh_token', res.data.refreshToken);
          localStorage.setItem('user', JSON.stringify(res.data.user));
          this.currentUser.set(res.data.user);
        }
      }));
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({ error: () => {} });
    this.clearStorage();
    this.currentUser.set(null);
    this.router.navigate(['/auth']);
  }

  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    const refreshToken = localStorage.getItem('refresh_token');
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/refresh`, { refreshToken })
      .pipe(tap(res => {
        if (res.success) {
          localStorage.setItem('access_token', res.data.accessToken);
          localStorage.setItem('refresh_token', res.data.refreshToken);
        }
      }));
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  private clearStorage(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user');
  }
}
TSEOF

cat > "$BASE/app/core/services/api.service.ts" << 'TSEOF'
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { ApiResponse, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(path: string, params?: Record<string, any>): Observable<ApiResponse<T>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined && params[key] !== '') {
          httpParams = httpParams.set(key, params[key].toString());
        }
      });
    }
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${path}`, { params: httpParams });
  }

  getPaginated<T>(path: string, params?: Record<string, any>): Observable<ApiResponse<PaginatedResult<T>>> {
    return this.get<PaginatedResult<T>>(path, params);
  }

  post<T>(path: string, body: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}/${path}`, body);
  }

  put<T>(path: string, body: any): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}/${path}`, body);
  }

  patch<T>(path: string, body: any): Observable<ApiResponse<T>> {
    return this.http.patch<ApiResponse<T>>(`${this.baseUrl}/${path}`, body);
  }

  delete<T>(path: string): Observable<ApiResponse<T>> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}/${path}`);
  }
}
TSEOF

cat > "$BASE/app/core/services/toast.service.ts" << 'TSEOF'
import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  duration: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private counter = 0;
  toasts = signal<Toast[]>([]);

  show(message: string, type: Toast['type'] = 'info', duration = 4000): void {
    const toast: Toast = { id: ++this.counter, message, type, duration };
    this.toasts.update(t => [...t, toast]);
    setTimeout(() => this.remove(toast.id), duration);
  }

  success(message: string): void { this.show(message, 'success'); }
  error(message: string): void { this.show(message, 'error', 6000); }
  warning(message: string): void { this.show(message, 'warning'); }
  info(message: string): void { this.show(message, 'info'); }

  remove(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }
}
TSEOF

# ============================================================
# CORE - INTERCEPTORS
# ============================================================
mkdir -p "$BASE/app/core/interceptors"

cat > "$BASE/app/core/interceptors/auth.interceptor.ts" << 'TSEOF'
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  return next(req);
};
TSEOF

cat > "$BASE/app/core/interceptors/error.interceptor.ts" << 'TSEOF'
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        localStorage.clear();
        router.navigate(['/auth']);
      } else if (error.status === 403) {
        toast.error('Accès refusé');
      } else if (error.status === 500) {
        toast.error('Erreur serveur. Veuillez réessayer.');
      } else if (error.status === 0) {
        toast.error('Connexion impossible au serveur');
      }
      return throwError(() => error);
    })
  );
};
TSEOF

# ============================================================
# CORE - GUARDS
# ============================================================
mkdir -p "$BASE/app/core/guards"

cat > "$BASE/app/core/guards/auth.guard.ts" << 'TSEOF'
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.isAuthenticated()) {
    return true;
  }
  router.navigate(['/auth']);
  return false;
};
TSEOF

echo "✅ Admin Panel Core files generated!"
