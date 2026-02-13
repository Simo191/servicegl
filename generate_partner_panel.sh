#!/bin/bash
set -e
PP="/home/claude/MultiServicesApp/src/Web/partner-panel"

# ============================================================
# CONFIG FILES
# ============================================================
cat > "$PP/package.json" << 'EOF'
{
  "name": "multiservices-partner-panel",
  "version": "1.0.0",
  "scripts": { "start": "ng serve --port 4201", "build": "ng build", "build:prod": "ng build --configuration production" },
  "private": true,
  "dependencies": {
    "@angular/animations": "^21.0.0", "@angular/common": "^21.0.0", "@angular/compiler": "^21.0.0",
    "@angular/core": "^21.0.0", "@angular/forms": "^21.0.0", "@angular/platform-browser": "^21.0.0",
    "@angular/platform-browser-dynamic": "^21.0.0", "@angular/router": "^21.0.0",
    "bootstrap": "^5.3.3", "bootstrap-icons": "^1.11.3", "chart.js": "^4.4.7",
    "rxjs": "~7.8.0", "tslib": "^2.7.0", "zone.js": "~0.15.0"
  },
  "devDependencies": { "@angular/build": "^21.0.0", "@angular/cli": "^21.0.0", "@angular/compiler-cli": "^21.0.0", "typescript": "~5.7.0" }
}
EOF

cat > "$PP/angular.json" << 'EOF'
{
  "$schema": "./node_modules/@angular/cli/lib/config/schema.json",
  "version": 1,
  "projects": {
    "partner-panel": {
      "projectType": "application", "root": "", "sourceRoot": "src", "prefix": "app",
      "architect": {
        "build": {
          "builder": "@angular/build:application",
          "options": {
            "outputPath": "dist/partner-panel", "index": "src/index.html", "browser": "src/main.ts",
            "polyfills": ["zone.js"], "tsConfig": "tsconfig.app.json",
            "styles": ["node_modules/bootstrap/dist/css/bootstrap.min.css","node_modules/bootstrap-icons/font/bootstrap-icons.css","src/styles.css"],
            "scripts": ["node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"]
          },
          "configurations": { "production": { "outputHashing": "all" }, "development": { "optimization": false, "sourceMap": true } },
          "defaultConfiguration": "production"
        },
        "serve": { "builder": "@angular/build:dev-server", "configurations": { "production": { "buildTarget": "partner-panel:build:production" }, "development": { "buildTarget": "partner-panel:build:development" } }, "defaultConfiguration": "development" }
      }
    }
  }
}
EOF

cat > "$PP/tsconfig.json" << 'EOF'
{
  "compilerOptions": { "outDir": "./dist/out-tsc", "strict": true, "sourceMap": true, "experimentalDecorators": true, "moduleResolution": "bundler", "target": "ES2022", "module": "ES2022", "lib": ["ES2022","dom"],
    "paths": { "@core/*": ["src/app/core/*"], "@shared/*": ["src/app/shared/*"], "@env/*": ["src/environments/*"] }
  },
  "angularCompilerOptions": { "strictTemplates": true }
}
EOF

cat > "$PP/tsconfig.app.json" << 'EOF'
{ "extends": "./tsconfig.json", "compilerOptions": { "outDir": "./out-tsc/app" }, "files": ["src/main.ts"], "include": ["src/**/*.d.ts"] }
EOF

# ============================================================
# SRC FILES
# ============================================================
mkdir -p "$PP/src/environments" "$PP/src/app/core/services" "$PP/src/app/core/models" "$PP/src/app/core/interceptors" "$PP/src/app/core/guards"
mkdir -p "$PP/src/app/layouts/partner-layout" "$PP/src/app/shared/components"
mkdir -p "$PP/src/app/features/auth" "$PP/src/app/features/dashboard" "$PP/src/app/features/orders" "$PP/src/app/features/menu" "$PP/src/app/features/services-mgmt" "$PP/src/app/features/catalog" "$PP/src/app/features/finances" "$PP/src/app/features/profile" "$PP/src/app/features/availability" "$PP/src/app/features/team" "$PP/src/app/features/stock"

# index.html
cat > "$PP/src/index.html" << 'EOF'
<!DOCTYPE html><html lang="fr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>MultiServices - Espace Partenaire</title></head><body><app-root></app-root></body></html>
EOF

# Copy admin styles (similar but different accent)
cp /home/claude/MultiServicesApp/src/Web/admin-panel/src/styles.css "$PP/src/styles.css"

cat > "$PP/src/environments/environment.ts" << 'EOF'
export const environment = { production: false, apiUrl: 'https://localhost:7001/api/v1', hubUrl: 'https://localhost:7001/hubs' };
EOF

cat > "$PP/src/main.ts" << 'EOF'
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
bootstrapApplication(AppComponent, appConfig).catch(err => console.error(err));
EOF

cat > "$PP/src/app/app.component.ts" << 'EOF'
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
@Component({ selector: 'app-root', standalone: true, imports: [RouterOutlet], template: '<router-outlet />' })
export class AppComponent {}
EOF

cat > "$PP/src/app/app.config.ts" << 'EOF'
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations()
  ]
};
EOF

cat > "$PP/src/app/app.routes.ts" << 'EOF'
import { Routes } from '@angular/router';
import { PartnerLayoutComponent } from './layouts/partner-layout/partner-layout.component';

export const routes: Routes = [
  { path: 'auth', loadComponent: () => import('./features/auth/login.component').then(m => m.PartnerLoginComponent) },
  {
    path: '', component: PartnerLayoutComponent, children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/partner-dashboard.component').then(m => m.PartnerDashboardComponent) },
      { path: 'orders', loadComponent: () => import('./features/orders/partner-orders.component').then(m => m.PartnerOrdersComponent) },
      { path: 'menu', loadComponent: () => import('./features/menu/menu-management.component').then(m => m.MenuManagementComponent) },
      { path: 'services', loadComponent: () => import('./features/services-mgmt/services-management.component').then(m => m.ServicesManagementComponent) },
      { path: 'catalog', loadComponent: () => import('./features/catalog/catalog-management.component').then(m => m.CatalogManagementComponent) },
      { path: 'stock', loadComponent: () => import('./features/stock/stock-management.component').then(m => m.StockManagementComponent) },
      { path: 'availability', loadComponent: () => import('./features/availability/availability.component').then(m => m.AvailabilityComponent) },
      { path: 'team', loadComponent: () => import('./features/team/team-management.component').then(m => m.TeamManagementComponent) },
      { path: 'finances', loadComponent: () => import('./features/finances/partner-finances.component').then(m => m.PartnerFinancesComponent) },
      { path: 'profile', loadComponent: () => import('./features/profile/partner-profile.component').then(m => m.PartnerProfileComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
EOF

# Core services - reuse similar patterns from admin
cat > "$PP/src/app/core/services/auth.service.ts" << 'EOF'
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '@env/environment';

interface PartnerUser { id: string; email: string; name: string; role: string; businessName: string; businessType: 'Restaurant' | 'ServiceProvider' | 'GroceryStore'; logoUrl: string; }
interface LoginResponse { accessToken: string; refreshToken: string; user: PartnerUser; }
interface ApiResp<T> { success: boolean; data: T; message: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  currentUser = signal<PartnerUser | null>(null);
  isAuthenticated = computed(() => !!this.currentUser());
  businessType = computed(() => this.currentUser()?.businessType || 'Restaurant');

  constructor(private http: HttpClient, private router: Router) {
    const u = localStorage.getItem('partner_user');
    if (u && localStorage.getItem('partner_token')) { try { this.currentUser.set(JSON.parse(u)); } catch { this.logout(); } }
  }

  login(email: string, password: string): Observable<ApiResp<LoginResponse>> {
    return this.http.post<ApiResp<LoginResponse>>(`${this.apiUrl}/partner-login`, { email, password })
      .pipe(tap(r => { if (r.success) { localStorage.setItem('partner_token', r.data.accessToken); localStorage.setItem('partner_refresh', r.data.refreshToken); localStorage.setItem('partner_user', JSON.stringify(r.data.user)); this.currentUser.set(r.data.user); } }));
  }

  logout(): void { localStorage.removeItem('partner_token'); localStorage.removeItem('partner_refresh'); localStorage.removeItem('partner_user'); this.currentUser.set(null); this.router.navigate(['/auth']); }
  getToken(): string | null { return localStorage.getItem('partner_token'); }
}
EOF

cat > "$PP/src/app/core/services/api.service.ts" << 'EOF'
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
@Injectable({ providedIn: 'root' })
export class ApiService {
  private baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}
  get<T>(path: string, params?: Record<string, any>): Observable<T> {
    let hp = new HttpParams();
    if (params) Object.keys(params).forEach(k => { if (params[k] != null && params[k] !== '') hp = hp.set(k, params[k]); });
    return this.http.get<T>(`${this.baseUrl}/${path}`, { params: hp });
  }
  post<T>(path: string, body: any): Observable<T> { return this.http.post<T>(`${this.baseUrl}/${path}`, body); }
  put<T>(path: string, body: any): Observable<T> { return this.http.put<T>(`${this.baseUrl}/${path}`, body); }
  patch<T>(path: string, body: any): Observable<T> { return this.http.patch<T>(`${this.baseUrl}/${path}`, body); }
  delete<T>(path: string): Observable<T> { return this.http.delete<T>(`${this.baseUrl}/${path}`); }
}
EOF

cat > "$PP/src/app/core/services/toast.service.ts" << 'EOF'
import { Injectable, signal } from '@angular/core';
export interface Toast { id: number; message: string; type: 'success'|'error'|'warning'|'info'; }
@Injectable({ providedIn: 'root' })
export class ToastService {
  private c = 0; toasts = signal<Toast[]>([]);
  show(msg: string, type: Toast['type'] = 'info', dur = 4000): void { const t = { id: ++this.c, message: msg, type }; this.toasts.update(a => [...a, t]); setTimeout(() => this.remove(t.id), dur); }
  success(m: string) { this.show(m,'success'); } error(m: string) { this.show(m,'error',6000); }
  warning(m: string) { this.show(m,'warning'); } info(m: string) { this.show(m,'info'); }
  remove(id: number) { this.toasts.update(a => a.filter(x => x.id !== id)); }
}
EOF

cat > "$PP/src/app/core/interceptors/auth.interceptor.ts" << 'EOF'
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const t = inject(AuthService).getToken();
  return next(t ? req.clone({ setHeaders: { Authorization: `Bearer ${t}` } }) : req);
};
EOF

cat > "$PP/src/app/core/guards/auth.guard.ts" << 'EOF'
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const authGuard: CanActivateFn = () => { const a = inject(AuthService); if (a.isAuthenticated()) return true; inject(Router).navigate(['/auth']); return false; };
EOF

# ============================================================
# LAYOUT
# ============================================================
cat > "$PP/src/app/layouts/partner-layout/partner-layout.component.ts" << 'TSEOF'
import { Component, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-partner-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="d-flex">
      <aside class="admin-sidebar" [class.show]="sidebarOpen()">
        <a routerLink="/dashboard" class="sidebar-brand">
          <i class="bi bi-building"></i>
          <span>{{ auth.currentUser()?.businessName || 'Partenaire' }}</span>
        </a>

        <div class="sidebar-section-title">Navigation</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/dashboard" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-speedometer2"></i> Dashboard
          </a>
          <a class="nav-link" routerLink="/orders" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-receipt"></i> Commandes
          </a>

          @if (isRestaurant()) {
            <a class="nav-link" routerLink="/menu" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-menu-button-wide"></i> Menu
            </a>
          }
          @if (isService()) {
            <a class="nav-link" routerLink="/services" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-tools"></i> Services
            </a>
            <a class="nav-link" routerLink="/availability" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-calendar3"></i> Disponibilités
            </a>
            <a class="nav-link" routerLink="/team" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-people"></i> Équipe
            </a>
          }
          @if (isGrocery()) {
            <a class="nav-link" routerLink="/catalog" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-box-seam"></i> Catalogue
            </a>
            <a class="nav-link" routerLink="/stock" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-clipboard-data"></i> Stock
            </a>
          }
        </nav>

        <div class="sidebar-section-title">Compte</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/finances" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-wallet2"></i> Finances
          </a>
          <a class="nav-link" routerLink="/profile" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-gear"></i> Profil
          </a>
          <a class="nav-link text-danger" (click)="auth.logout()" style="cursor:pointer">
            <i class="bi bi-box-arrow-right"></i> Déconnexion
          </a>
        </nav>
      </aside>

      <main class="admin-main flex-grow-1">
        <header class="admin-header">
          <button class="btn btn-link d-lg-none p-0 text-dark" (click)="toggleSidebar()"><i class="bi bi-list fs-4"></i></button>
          <div class="d-flex align-items-center gap-2">
            <span class="badge" [class]="typeClass()">{{ typeLabel() }}</span>
            <span class="fw-medium d-none d-md-inline">{{ auth.currentUser()?.name }}</span>
          </div>
        </header>
        <div class="admin-content"><router-outlet /></div>
      </main>
    </div>

    @if (sidebarOpen()) {
      <div class="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-50 d-lg-none" style="z-index:1025" (click)="closeMobile()"></div>
    }
  `
})
export class PartnerLayoutComponent {
  sidebarOpen = signal(false);
  constructor(public auth: AuthService) {}

  isRestaurant = computed(() => this.auth.businessType() === 'Restaurant');
  isService = computed(() => this.auth.businessType() === 'ServiceProvider');
  isGrocery = computed(() => this.auth.businessType() === 'GroceryStore');
  typeClass = computed(() => { const t = this.auth.businessType(); return t === 'Restaurant' ? 'bg-warning' : t === 'ServiceProvider' ? 'bg-info' : 'bg-success'; });
  typeLabel = computed(() => { const t = this.auth.businessType(); return t === 'Restaurant' ? '🍔 Restaurant' : t === 'ServiceProvider' ? '🛠️ Services' : '🛒 Magasin'; });

  toggleSidebar() { this.sidebarOpen.update(v => !v); }
  closeMobile() { this.sidebarOpen.set(false); }
}
TSEOF

# ============================================================
# FEATURES
# ============================================================

# Auth
cat > "$PP/src/app/features/auth/login.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center" style="background:linear-gradient(135deg,#0ea5e9,#4f46e5)">
      <div class="card border-0 shadow-lg" style="width:420px;border-radius:16px">
        <div class="card-body p-5">
          <div class="text-center mb-4">
            <div class="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 rounded-circle mb-3" style="width:64px;height:64px">
              <i class="bi bi-building text-primary fs-3"></i>
            </div>
            <h4 class="fw-bold">Espace Partenaire</h4>
            <p class="text-muted">Restaurant • Service • Magasin</p>
          </div>
          <div class="mb-3">
            <label class="form-label fw-medium">Email</label>
            <input type="email" class="form-control" [(ngModel)]="email" placeholder="partenaire@email.com" />
          </div>
          <div class="mb-4">
            <label class="form-label fw-medium">Mot de passe</label>
            <input type="password" class="form-control" [(ngModel)]="password" placeholder="••••••••" />
          </div>
          <button class="btn btn-primary w-100 py-2 fw-medium" [disabled]="loading()" (click)="login()">
            @if (loading()) { <span class="spinner-border spinner-border-sm me-2"></span> }
            Se connecter
          </button>
        </div>
      </div>
    </div>
  `
})
export class PartnerLoginComponent {
  email = ''; password = ''; loading = signal(false);
  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}
  login(): void {
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: r => { if (r.success) { this.router.navigate(['/dashboard']); } else { this.toast.error(r.message); } this.loading.set(false); },
      error: () => { this.toast.error('Identifiants invalides'); this.loading.set(false); }
    });
  }
}
TSEOF

# Dashboard
cat > "$PP/src/app/features/dashboard/partner-dashboard.component.ts" << 'TSEOF'
import { Component, OnInit, signal, viewChild, ElementRef, AfterViewInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-partner-dashboard',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <div class="mb-4">
      <h4 class="fw-bold mb-1">Bonjour, {{ auth.currentUser()?.name }} 👋</h4>
      <p class="text-muted mb-0">Voici le résumé de votre activité</p>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">CA du jour</p>
          <h3 class="stat-value">{{ todayRevenue() | number:'1.0-0' }} <small class="fs-6 fw-normal">MAD</small></h3>
          <span class="stat-change text-success"><i class="bi bi-arrow-up"></i> +12%</span>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Commandes du jour</p>
          <h3 class="stat-value">{{ todayOrders() }}</h3>
          <span class="stat-change text-success"><i class="bi bi-arrow-up"></i> +8%</span>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Note moyenne</p>
          <h3 class="stat-value">4.6 <small class="fs-6 text-warning">★</small></h3>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Commandes en cours</p>
          <h3 class="stat-value text-primary">{{ activeOrders() }}</h3>
        </div>
      </div>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-lg-8">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Revenus - 7 derniers jours</h6></div>
          <div class="p-3"><canvas #weekChart height="280"></canvas></div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Commandes récentes</h6></div>
          <div class="list-group list-group-flush">
            @for (o of recentOrders(); track o.id) {
              <div class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div class="fw-medium small">#{{ o.number }}</div>
                  <small class="text-muted">{{ o.customer }}</small>
                </div>
                <div class="text-end">
                  <div class="fw-bold small">{{ o.amount }} MAD</div>
                  <span class="badge" [class]="o.statusClass">{{ o.statusLabel }}</span>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  `
})
export class PartnerDashboardComponent implements OnInit, AfterViewInit {
  weekChartRef = viewChild<ElementRef>('weekChart');
  todayRevenue = signal(4250); todayOrders = signal(28); activeOrders = signal(5);
  recentOrders = signal([
    { id: '1', number: 'CMD-452', customer: 'Ahmed B.', amount: 185, statusLabel: 'En cours', statusClass: 'bg-primary' },
    { id: '2', number: 'CMD-451', customer: 'Sara L.', amount: 98, statusLabel: 'Prête', statusClass: 'bg-success' },
    { id: '3', number: 'CMD-450', customer: 'Omar K.', amount: 245, statusLabel: 'Livrée', statusClass: 'bg-secondary' },
    { id: '4', number: 'CMD-449', customer: 'Fatima Z.', amount: 132, statusLabel: 'Livrée', statusClass: 'bg-secondary' },
  ]);

  constructor(public auth: AuthService, private api: ApiService) {}
  ngOnInit(): void {}
  ngAfterViewInit(): void { setTimeout(() => this.initChart(), 200); }

  private initChart(): void {
    const el = this.weekChartRef()?.nativeElement;
    if (!el) return;
    new Chart(el, {
      type: 'bar',
      data: {
        labels: ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'],
        datasets: [{ label: 'CA (MAD)', data: [3200, 4100, 3800, 5200, 6100, 4800, 3500], backgroundColor: 'rgba(79,70,229,0.2)', borderColor: '#4f46e5', borderWidth: 2, borderRadius: 8 }]
      },
      options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
    });
  }
}
TSEOF

# Orders
cat > "$PP/src/app/features/orders/partner-orders.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-orders',
  standalone: true,
  imports: [FormsModule, DecimalPipe, DatePipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mes Commandes</h4>
      <div class="btn-group">
        @for (tab of tabs; track tab.value) {
          <button class="btn btn-sm" [class.btn-primary]="selectedTab === tab.value" [class.btn-outline-primary]="selectedTab !== tab.value" (click)="selectedTab = tab.value">
            {{ tab.label }} ({{ tab.count }})
          </button>
        }
      </div>
    </div>

    <div class="row g-3">
      @for (order of filteredOrders(); track order.id) {
        <div class="col-12">
          <div class="table-card">
            <div class="card-body p-3">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <h6 class="fw-bold mb-1">#{{ order.number }} - {{ order.customer }}</h6>
                  <p class="text-muted small mb-1">{{ order.items }}</p>
                  <span class="badge" [class]="order.statusClass">{{ order.statusLabel }}</span>
                </div>
                <div class="text-end">
                  <h5 class="fw-bold text-primary mb-1">{{ order.amount | number:'1.2-2' }} MAD</h5>
                  <small class="text-muted">{{ order.time }}</small>
                </div>
              </div>
              @if (order.status === 'pending') {
                <div class="mt-3 d-flex gap-2">
                  <button class="btn btn-success btn-sm" (click)="acceptOrder(order)"><i class="bi bi-check-lg me-1"></i>Accepter</button>
                  <button class="btn btn-outline-danger btn-sm" (click)="rejectOrder(order)"><i class="bi bi-x-lg me-1"></i>Refuser</button>
                </div>
              }
              @if (order.status === 'preparing') {
                <div class="mt-3">
                  <button class="btn btn-primary btn-sm" (click)="markReady(order)"><i class="bi bi-check-circle me-1"></i>Marquer prête</button>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class PartnerOrdersComponent {
  selectedTab = 'all';
  tabs = [
    { value: 'all', label: 'Toutes', count: 12 },
    { value: 'pending', label: 'Nouvelles', count: 3 },
    { value: 'preparing', label: 'En cours', count: 4 },
    { value: 'ready', label: 'Prêtes', count: 2 },
    { value: 'delivered', label: 'Livrées', count: 3 },
  ];

  orders = signal([
    { id: '1', number: 'CMD-456', customer: 'Ahmed B.', items: '2x Tajine, 1x Couscous, 2x Thé', amount: 285, status: 'pending', statusLabel: 'Nouvelle', statusClass: 'bg-warning', time: 'Il y a 2 min' },
    { id: '2', number: 'CMD-455', customer: 'Sara L.', items: '1x Pizza Margherita, 1x Tiramisu', amount: 135, status: 'pending', statusLabel: 'Nouvelle', statusClass: 'bg-warning', time: 'Il y a 5 min' },
    { id: '3', number: 'CMD-454', customer: 'Omar K.', items: '3x Burger, 3x Frites, 3x Coca', amount: 198, status: 'preparing', statusLabel: 'En préparation', statusClass: 'bg-primary', time: 'Il y a 15 min' },
    { id: '4', number: 'CMD-453', customer: 'Youssef M.', items: '1x Pastilla, 2x Brochettes', amount: 210, status: 'preparing', statusLabel: 'En préparation', statusClass: 'bg-primary', time: 'Il y a 22 min' },
    { id: '5', number: 'CMD-452', customer: 'Fatima Z.', items: '2x Sushi Roll, 1x Miso Soup', amount: 320, status: 'ready', statusLabel: 'Prête', statusClass: 'bg-success', time: 'Il y a 30 min' },
    { id: '6', number: 'CMD-451', customer: 'Karim T.', items: '1x Tajine Poulet', amount: 95, status: 'delivered', statusLabel: 'Livrée', statusClass: 'bg-secondary', time: 'Il y a 1h' },
  ]);

  constructor(private toast: ToastService) {}

  filteredOrders() {
    if (this.selectedTab === 'all') return this.orders();
    return this.orders().filter(o => o.status === this.selectedTab);
  }

  acceptOrder(o: any): void { o.status = 'preparing'; o.statusLabel = 'En préparation'; o.statusClass = 'bg-primary'; this.toast.success('Commande acceptée'); }
  rejectOrder(o: any): void { this.orders.update(list => list.filter(x => x.id !== o.id)); this.toast.warning('Commande refusée'); }
  markReady(o: any): void { o.status = 'ready'; o.statusLabel = 'Prête'; o.statusClass = 'bg-success'; this.toast.success('Commande marquée prête'); }
}
TSEOF

# Menu Management (Restaurant)
cat > "$PP/src/app/features/menu/menu-management.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-menu-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Gestion du Menu</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Catégorie</button>
        <button class="btn btn-primary btn-sm" (click)="showAddItem.set(true)"><i class="bi bi-plus-lg me-1"></i>Plat</button>
      </div>
    </div>

    @if (showAddItem()) {
      <div class="table-card mb-4">
        <div class="card-header"><h6 class="fw-bold mb-0">Ajouter un plat</h6></div>
        <div class="card-body p-3">
          <div class="row g-3">
            <div class="col-md-4"><label class="form-label small">Nom</label><input class="form-control" [(ngModel)]="newItem.name" /></div>
            <div class="col-md-3"><label class="form-label small">Catégorie</label>
              <select class="form-select" [(ngModel)]="newItem.category">
                @for (cat of categories; track cat) { <option [value]="cat">{{ cat }}</option> }
              </select>
            </div>
            <div class="col-md-2"><label class="form-label small">Prix (MAD)</label><input type="number" class="form-control" [(ngModel)]="newItem.price" /></div>
            <div class="col-md-3"><label class="form-label small">Description</label><input class="form-control" [(ngModel)]="newItem.description" /></div>
            <div class="col-12">
              <button class="btn btn-primary btn-sm me-2" (click)="addItem()">Ajouter</button>
              <button class="btn btn-light btn-sm" (click)="showAddItem.set(false)">Annuler</button>
            </div>
          </div>
        </div>
      </div>
    }

    @for (cat of categories; track cat) {
      <div class="table-card mb-3">
        <div class="card-header">
          <h6 class="fw-bold mb-0">{{ cat }}</h6>
          <span class="badge bg-light text-dark">{{ itemsByCategory(cat).length }} plats</span>
        </div>
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead><tr><th>Plat</th><th>Description</th><th>Prix</th><th>Disponible</th><th class="text-end">Actions</th></tr></thead>
            <tbody>
              @for (item of itemsByCategory(cat); track item.id) {
                <tr>
                  <td class="fw-medium">{{ item.name }}</td>
                  <td class="text-muted small">{{ item.description }}</td>
                  <td class="fw-bold">{{ item.price | number:'1.2-2' }} MAD</td>
                  <td>
                    <div class="form-check form-switch">
                      <input class="form-check-input" type="checkbox" [checked]="item.available" (change)="toggleAvailability(item)">
                    </div>
                  </td>
                  <td class="text-end">
                    <button class="btn btn-sm btn-outline-primary me-1"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    }
  `
})
export class MenuManagementComponent {
  showAddItem = signal(false);
  categories = ['Entrées', 'Plats principaux', 'Desserts', 'Boissons'];
  newItem = { name: '', category: 'Entrées', price: 0, description: '' };

  items = signal([
    { id: '1', name: 'Harira', category: 'Entrées', description: 'Soupe traditionnelle', price: 25, available: true },
    { id: '2', name: 'Briouates', category: 'Entrées', description: 'Briouates au fromage', price: 35, available: true },
    { id: '3', name: 'Tajine Poulet', category: 'Plats principaux', description: 'Poulet aux olives et citron', price: 85, available: true },
    { id: '4', name: 'Couscous Royal', category: 'Plats principaux', description: '7 légumes, agneau', price: 120, available: true },
    { id: '5', name: 'Pastilla', category: 'Plats principaux', description: 'Au pigeon', price: 95, available: false },
    { id: '6', name: 'Corne de gazelle', category: 'Desserts', description: 'Pâtisserie amande', price: 30, available: true },
    { id: '7', name: 'Thé à la menthe', category: 'Boissons', description: 'Thé vert menthe', price: 15, available: true },
    { id: '8', name: 'Jus d\'orange frais', category: 'Boissons', description: 'Pressé maison', price: 20, available: true },
  ]);

  constructor(private toast: ToastService) {}
  itemsByCategory(cat: string) { return this.items().filter(i => i.category === cat); }
  toggleAvailability(item: any) { item.available = !item.available; this.toast.info(item.available ? 'Plat activé' : 'Plat désactivé'); }
  addItem(): void { this.items.update(list => [...list, { ...this.newItem, id: Date.now().toString(), available: true }]); this.showAddItem.set(false); this.toast.success('Plat ajouté'); }
}
TSEOF

# Services Management (ServiceProvider)
cat > "$PP/src/app/features/services-mgmt/services-management.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-services-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mes Services</h4>
      <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Ajouter un service</button>
    </div>
    <div class="row g-3">
      @for (s of services(); track s.id) {
        <div class="col-md-6">
          <div class="table-card">
            <div class="card-body p-3">
              <div class="d-flex justify-content-between">
                <h6 class="fw-bold mb-1">{{ s.name }}</h6>
                <div class="form-check form-switch">
                  <input class="form-check-input" type="checkbox" [checked]="s.active">
                </div>
              </div>
              <p class="text-muted small mb-2">{{ s.description }}</p>
              <div class="d-flex gap-3 small">
                <span><i class="bi bi-clock me-1"></i>{{ s.duration }}</span>
                <span class="fw-bold text-primary">
                  @if (s.priceType === 'hourly') { {{ s.price | number }} MAD/h }
                  @else if (s.priceType === 'fixed') { {{ s.price | number }} MAD }
                  @else { Sur devis }
                </span>
              </div>
              <div class="mt-2">
                <button class="btn btn-sm btn-outline-primary me-1"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class ServicesManagementComponent {
  services = signal([
    { id: '1', name: 'Réparation fuite', description: 'Détection et réparation de fuites d\'eau', duration: '1-3h', price: 200, priceType: 'hourly', active: true },
    { id: '2', name: 'Débouchage canalisation', description: 'Débouchage professionnel', duration: '1-2h', price: 350, priceType: 'fixed', active: true },
    { id: '3', name: 'Installation sanitaire', description: 'Installation complète salle de bain', duration: '4-8h', price: 0, priceType: 'quote', active: true },
    { id: '4', name: 'Réparation chauffe-eau', description: 'Diagnostic et réparation', duration: '1-3h', price: 250, priceType: 'fixed', active: false },
  ]);
}
TSEOF

# Catalog Management (Grocery)
cat > "$PP/src/app/features/catalog/catalog-management.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Catalogue Produits</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm"><i class="bi bi-upload me-1"></i>Import CSV</button>
        <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Produit</button>
      </div>
    </div>

    <div class="table-card mb-3">
      <div class="card-header">
        <div class="input-group" style="max-width:300px">
          <span class="input-group-text"><i class="bi bi-search"></i></span>
          <input type="text" class="form-control" placeholder="Rechercher un produit..." [(ngModel)]="search" />
        </div>
        <select class="form-select" style="width:auto" [(ngModel)]="categoryFilter">
          <option value="">Tous les rayons</option>
          @for (cat of departments; track cat) { <option [value]="cat">{{ cat }}</option> }
        </select>
      </div>
    </div>

    <div class="table-card">
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Produit</th><th>Rayon</th><th>Code-barre</th><th>Prix</th><th>Stock</th><th>Statut</th><th class="text-end">Actions</th></tr></thead>
          <tbody>
            @for (p of filteredProducts(); track p.id) {
              <tr>
                <td class="fw-medium">{{ p.name }}</td>
                <td><span class="badge bg-light text-dark">{{ p.department }}</span></td>
                <td class="font-monospace small">{{ p.barcode }}</td>
                <td class="fw-bold">{{ p.price | number:'1.2-2' }} MAD</td>
                <td [class.text-danger]="p.stock < 10" [class.fw-bold]="p.stock < 10">{{ p.stock }}</td>
                <td>
                  @if (p.stock > 0) { <span class="badge bg-success-subtle text-success">En stock</span> }
                  @else { <span class="badge bg-danger-subtle text-danger">Rupture</span> }
                </td>
                <td class="text-end">
                  <button class="btn btn-sm btn-outline-primary me-1"><i class="bi bi-pencil"></i></button>
                  <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class CatalogManagementComponent {
  search = ''; categoryFilter = '';
  departments = ['Fruits & Légumes', 'Viandes & Poissons', 'Épicerie', 'Boissons', 'Produits laitiers', 'Hygiène & Beauté'];

  products = signal([
    { id: '1', name: 'Tomates rondes 1kg', department: 'Fruits & Légumes', barcode: '6111234567890', price: 12.90, stock: 150 },
    { id: '2', name: 'Poulet fermier', department: 'Viandes & Poissons', barcode: '6111234567891', price: 65.00, stock: 45 },
    { id: '3', name: 'Lait Centrale 1L', department: 'Produits laitiers', barcode: '6111234567892', price: 7.50, stock: 200 },
    { id: '4', name: 'Huile d\'olive 1L', department: 'Épicerie', barcode: '6111234567893', price: 55.00, stock: 8 },
    { id: '5', name: 'Eau Sidi Ali 1.5L', department: 'Boissons', barcode: '6111234567894', price: 5.50, stock: 0 },
    { id: '6', name: 'Shampoing Pantene', department: 'Hygiène & Beauté', barcode: '6111234567895', price: 42.00, stock: 35 },
  ]);

  filteredProducts() {
    let list = this.products();
    if (this.search) list = list.filter(p => p.name.toLowerCase().includes(this.search.toLowerCase()));
    if (this.categoryFilter) list = list.filter(p => p.department === this.categoryFilter);
    return list;
  }
}
TSEOF

# Stock Management (Grocery)
cat > "$PP/src/app/features/stock/stock-management.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
@Component({
  selector: 'app-stock-management',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <h4 class="fw-bold mb-4">Gestion du Stock</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-primary">12,500</h3><small class="text-muted">Produits total</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-success">11,850</h3><small class="text-muted">En stock</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-warning">42</h3><small class="text-muted">Stock faible</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-danger">15</h3><small class="text-muted">Rupture</small></div></div>
    </div>
    <div class="table-card">
      <div class="card-header"><h6 class="fw-bold mb-0">⚠️ Alertes stock</h6></div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Produit</th><th>Stock actuel</th><th>Seuil alerte</th><th>Statut</th><th class="text-end">Action</th></tr></thead>
          <tbody>
            @for (a of alerts(); track a.name) {
              <tr>
                <td class="fw-medium">{{ a.name }}</td>
                <td [class.text-danger]="a.stock === 0" class="fw-bold">{{ a.stock }}</td>
                <td>{{ a.threshold }}</td>
                <td>@if (a.stock === 0) { <span class="badge bg-danger">Rupture</span> } @else { <span class="badge bg-warning">Faible</span> }</td>
                <td class="text-end"><button class="btn btn-sm btn-outline-primary">Réapprovisionner</button></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class StockManagementComponent {
  alerts = signal([
    { name: 'Eau Sidi Ali 1.5L', stock: 0, threshold: 50 },
    { name: 'Huile d\'olive 1L', stock: 8, threshold: 20 },
    { name: 'Beurre Président', stock: 5, threshold: 15 },
    { name: 'Oeufs (douzaine)', stock: 12, threshold: 30 },
  ]);
}
TSEOF

# Availability (ServiceProvider)
cat > "$PP/src/app/features/availability/availability.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h4 class="fw-bold mb-4">Gestion des Disponibilités</h4>
    <div class="table-card mb-4">
      <div class="card-header"><h6 class="fw-bold mb-0">Horaires de travail</h6></div>
      <div class="card-body p-3">
        @for (day of days; track day.name) {
          <div class="d-flex align-items-center gap-3 py-2 border-bottom">
            <div style="width:100px" class="fw-medium">{{ day.name }}</div>
            <div class="form-check form-switch">
              <input class="form-check-input" type="checkbox" [(ngModel)]="day.active">
            </div>
            @if (day.active) {
              <input type="time" class="form-control form-control-sm" style="width:120px" [(ngModel)]="day.start" />
              <span>→</span>
              <input type="time" class="form-control form-control-sm" style="width:120px" [(ngModel)]="day.end" />
            } @else {
              <span class="text-muted small">Indisponible</span>
            }
          </div>
        }
        <button class="btn btn-primary btn-sm mt-3" (click)="save()"><i class="bi bi-check-lg me-1"></i>Sauvegarder</button>
      </div>
    </div>

    <div class="table-card">
      <div class="card-header">
        <h6 class="fw-bold mb-0">Mode indisponible</h6>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" [(ngModel)]="unavailable">
          <label class="form-check-label">{{ unavailable ? 'Indisponible' : 'Disponible' }}</label>
        </div>
      </div>
    </div>
  `
})
export class AvailabilityComponent {
  unavailable = false;
  days = [
    { name: 'Lundi', active: true, start: '08:00', end: '18:00' },
    { name: 'Mardi', active: true, start: '08:00', end: '18:00' },
    { name: 'Mercredi', active: true, start: '08:00', end: '18:00' },
    { name: 'Jeudi', active: true, start: '08:00', end: '18:00' },
    { name: 'Vendredi', active: true, start: '08:00', end: '18:00' },
    { name: 'Samedi', active: true, start: '09:00', end: '14:00' },
    { name: 'Dimanche', active: false, start: '', end: '' },
  ];
  constructor(private toast: ToastService) {}
  save(): void { this.toast.success('Horaires sauvegardés'); }
}
TSEOF

# Team Management (ServiceProvider)
cat > "$PP/src/app/features/team/team-management.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
@Component({
  selector: 'app-team-management',
  standalone: true,
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mon Équipe</h4>
      <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Ajouter un intervenant</button>
    </div>
    <div class="row g-3">
      @for (m of members(); track m.id) {
        <div class="col-md-4">
          <div class="table-card text-center p-4">
            <div class="rounded-circle bg-primary bg-opacity-10 text-primary d-inline-flex align-items-center justify-content-center mb-2" style="width:60px;height:60px;font-size:1.2rem;font-weight:600">{{ m.initials }}</div>
            <h6 class="fw-bold mb-0">{{ m.name }}</h6>
            <small class="text-muted">{{ m.role }}</small>
            <div class="mt-2">
              @if (m.available) { <span class="badge bg-success">Disponible</span> }
              @else { <span class="badge bg-secondary">Occupé</span> }
            </div>
            <div class="mt-2 small text-muted">{{ m.interventions }} interventions • {{ m.rating }} ★</div>
          </div>
        </div>
      }
    </div>
  `
})
export class TeamManagementComponent {
  members = signal([
    { id: '1', name: 'Rachid M.', initials: 'RM', role: 'Plombier senior', available: true, interventions: 180, rating: 4.8 },
    { id: '2', name: 'Samir B.', initials: 'SB', role: 'Plombier', available: false, interventions: 95, rating: 4.5 },
    { id: '3', name: 'Hassan K.', initials: 'HK', role: 'Apprenti', available: true, interventions: 30, rating: 4.2 },
  ]);
}
TSEOF

# Finances
cat > "$PP/src/app/features/finances/partner-finances.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
@Component({
  selector: 'app-partner-finances',
  standalone: true,
  imports: [DecimalPipe, DatePipe],
  template: `
    <h4 class="fw-bold mb-4">Mes Finances</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Solde disponible</p><h3 class="stat-value text-success">{{ balance() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">CA ce mois</p><h3 class="stat-value">{{ monthRevenue() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Commissions</p><h3 class="stat-value text-danger">-{{ commissions() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Total viré</p><h3 class="stat-value">{{ totalPaid() | number:'1.0-0' }} MAD</h3></div></div>
    </div>
    <div class="d-flex gap-2 mb-4">
      <button class="btn btn-primary"><i class="bi bi-send me-2"></i>Demander un virement</button>
      <button class="btn btn-outline-primary"><i class="bi bi-download me-2"></i>Export Excel</button>
    </div>
    <div class="table-card">
      <div class="card-header"><h6 class="fw-bold mb-0">Historique des paiements</h6></div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Date</th><th>Référence</th><th>Type</th><th>Montant</th><th>Statut</th></tr></thead>
          <tbody>
            @for (p of payments(); track p.ref) {
              <tr>
                <td class="text-muted">{{ p.date }}</td>
                <td class="fw-medium">{{ p.ref }}</td>
                <td>{{ p.type }}</td>
                <td class="fw-bold" [class.text-success]="p.amount > 0" [class.text-danger]="p.amount < 0">{{ p.amount > 0 ? '+' : '' }}{{ p.amount | number:'1.2-2' }} MAD</td>
                <td><span class="badge" [class]="p.statusClass">{{ p.status }}</span></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class PartnerFinancesComponent {
  balance = signal(15250); monthRevenue = signal(42000); commissions = signal(6300); totalPaid = signal(128000);
  payments = signal([
    { date: '02/06/2025', ref: 'VIR-1045', type: 'Virement', amount: 8500, status: 'Effectué', statusClass: 'bg-success' },
    { date: '25/05/2025', ref: 'VIR-1038', type: 'Virement', amount: 12000, status: 'Effectué', statusClass: 'bg-success' },
    { date: '02/06/2025', ref: 'COM-8854', type: 'Commission', amount: -27.83, status: 'Prélevée', statusClass: 'bg-secondary' },
    { date: '01/06/2025', ref: 'COM-8850', type: 'Commission', amount: -35.25, status: 'Prélevée', statusClass: 'bg-secondary' },
  ]);
}
TSEOF

# Profile
cat > "$PP/src/app/features/profile/partner-profile.component.ts" << 'TSEOF'
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-profile',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h4 class="fw-bold mb-4">Mon Profil</h4>
    <div class="table-card">
      <div class="card-body p-4">
        <div class="row g-3">
          <div class="col-md-6"><label class="form-label fw-medium">Nom de l'entreprise</label><input class="form-control" [value]="auth.currentUser()?.businessName" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Responsable</label><input class="form-control" [value]="auth.currentUser()?.name" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Email</label><input class="form-control" [value]="auth.currentUser()?.email" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Téléphone</label><input class="form-control" value="+212600000000" /></div>
          <div class="col-12"><label class="form-label fw-medium">Adresse</label><input class="form-control" value="12 Rue Mohammed V, Casablanca" /></div>
          <div class="col-12"><label class="form-label fw-medium">Description</label><textarea class="form-control" rows="3">Restaurant traditionnel marocain depuis 2005.</textarea></div>
          <div class="col-12"><button class="btn btn-primary" (click)="toast.success('Profil mis à jour')"><i class="bi bi-check-lg me-2"></i>Sauvegarder</button></div>
        </div>
      </div>
    </div>
  `
})
export class PartnerProfileComponent {
  constructor(public auth: AuthService, public toast: ToastService) {}
}
TSEOF

echo "✅ Partner Panel complete!"
