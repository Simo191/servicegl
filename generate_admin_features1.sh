#!/bin/bash
set -e
BASE="/home/claude/MultiServicesApp/src/Web/admin-panel/src/app/features"

# ============================================================
# AUTH - LOGIN
# ============================================================
mkdir -p "$BASE/auth/login"

cat > "$BASE/auth/login/login.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center" style="background:linear-gradient(135deg,#667eea 0%,#764ba2 100%)">
      <div class="card border-0 shadow-lg" style="width:420px;border-radius:16px">
        <div class="card-body p-5">
          <div class="text-center mb-4">
            <div class="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 rounded-circle mb-3" style="width:64px;height:64px">
              <i class="bi bi-grid-1x2-fill text-primary fs-3"></i>
            </div>
            <h4 class="fw-bold">MultiServices Admin</h4>
            <p class="text-muted">Connectez-vous à votre compte</p>
          </div>

          <div class="mb-3">
            <label class="form-label fw-medium">Email</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-envelope"></i></span>
              <input type="email" class="form-control" [(ngModel)]="email" placeholder="admin@multiservices.ma" />
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-medium">Mot de passe</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-lock"></i></span>
              <input [type]="showPass() ? 'text' : 'password'" class="form-control" [(ngModel)]="password" placeholder="••••••••" />
              <button class="btn btn-outline-secondary" type="button" (click)="showPass.set(!showPass())">
                <i class="bi" [class.bi-eye]="!showPass()" [class.bi-eye-slash]="showPass()"></i>
              </button>
            </div>
          </div>

          <button class="btn btn-primary w-100 py-2 fw-medium" [disabled]="loading()" (click)="login()">
            @if (loading()) {
              <span class="spinner-border spinner-border-sm me-2"></span>
            }
            Se connecter
          </button>

          <div class="text-center mt-3">
            <a href="#" class="text-muted small text-decoration-none">Mot de passe oublié ?</a>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  loading = signal(false);
  showPass = signal(false);

  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}

  login(): void {
    if (!this.email || !this.password) {
      this.toast.warning('Veuillez remplir tous les champs');
      return;
    }
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Connexion réussie !');
          this.router.navigate(['/dashboard']);
        } else {
          this.toast.error(res.message || 'Identifiants invalides');
        }
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Identifiants invalides');
        this.loading.set(false);
      }
    });
  }
}
TSEOF

# ============================================================
# DASHBOARD
# ============================================================
mkdir -p "$BASE/dashboard"

cat > "$BASE/dashboard/dashboard.component.ts" << 'TSEOF'
import { Component, OnInit, signal, ElementRef, viewChild, AfterViewInit } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { DashboardStats } from '../../core/models/api.models';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DecimalPipe, DatePipe, RouterLink, StatCardComponent, LoadingSpinnerComponent, StatusBadgePipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Dashboard</h4>
        <p class="text-muted mb-0">Vue d'ensemble de la plateforme</p>
      </div>
      <div class="d-flex gap-2">
        <select class="form-select form-select-sm" style="width:auto" (change)="onPeriodChange($event)">
          <option value="today">Aujourd'hui</option>
          <option value="week" selected>Cette semaine</option>
          <option value="month">Ce mois</option>
          <option value="year">Cette année</option>
        </select>
      </div>
    </div>

    @if (loading()) {
      <app-loading />
    } @else {
      <!-- KPI Cards -->
      <div class="row g-3 mb-4">
        <div class="col-xl-3 col-md-6">
          <app-stat-card label="Chiffre d'affaires" [value]="stats().totalRevenue" [change]="stats().revenueChange"
            icon="bi-currency-dollar" bgColor="#ecfdf5" iconColor="#10b981" [isCurrency]="true" />
        </div>
        <div class="col-xl-3 col-md-6">
          <app-stat-card label="Commandes totales" [value]="stats().totalOrders" [change]="stats().ordersChange"
            icon="bi-receipt" bgColor="#eef2ff" iconColor="#4f46e5" />
        </div>
        <div class="col-xl-3 col-md-6">
          <app-stat-card label="Clients actifs" [value]="stats().activeClients" [change]="stats().clientsChange"
            icon="bi-people" bgColor="#fef3c7" iconColor="#f59e0b" />
        </div>
        <div class="col-xl-3 col-md-6">
          <app-stat-card label="Prestataires actifs" [value]="stats().activeProviders" [change]="stats().providersChange"
            icon="bi-building" bgColor="#fce7f3" iconColor="#ec4899" />
        </div>
      </div>

      <!-- Module KPIs -->
      <div class="row g-3 mb-4">
        <div class="col-md-4">
          <div class="stat-card border-start border-4 border-warning">
            <div class="d-flex align-items-center gap-3">
              <i class="bi bi-shop text-warning fs-3"></i>
              <div>
                <p class="stat-label mb-0">Commandes Restaurant</p>
                <h4 class="fw-bold mb-0">{{ stats().restaurantOrders | number }}</h4>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="stat-card border-start border-4 border-info">
            <div class="d-flex align-items-center gap-3">
              <i class="bi bi-tools text-info fs-3"></i>
              <div>
                <p class="stat-label mb-0">Interventions Services</p>
                <h4 class="fw-bold mb-0">{{ stats().serviceInterventions | number }}</h4>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="stat-card border-start border-4 border-success">
            <div class="d-flex align-items-center gap-3">
              <i class="bi bi-cart3 text-success fs-3"></i>
              <div>
                <p class="stat-label mb-0">Commandes Courses</p>
                <h4 class="fw-bold mb-0">{{ stats().groceryOrders | number }}</h4>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts -->
      <div class="row g-3 mb-4">
        <div class="col-lg-8">
          <div class="table-card">
            <div class="card-header">
              <h6 class="fw-bold mb-0">Chiffre d'affaires</h6>
            </div>
            <div class="p-3">
              <canvas #revenueChart height="280"></canvas>
            </div>
          </div>
        </div>
        <div class="col-lg-4">
          <div class="table-card">
            <div class="card-header">
              <h6 class="fw-bold mb-0">Répartition des commandes</h6>
            </div>
            <div class="p-3">
              <canvas #ordersChart height="280"></canvas>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Orders -->
      <div class="table-card">
        <div class="card-header">
          <h6 class="fw-bold mb-0">Commandes récentes</h6>
          <a routerLink="/orders" class="btn btn-sm btn-outline-primary">Voir tout</a>
        </div>
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>N° Commande</th>
                <th>Type</th>
                <th>Client</th>
                <th>Montant</th>
                <th>Statut</th>
                <th>Date</th>
              </tr>
            </thead>
            <tbody>
              @for (order of stats().recentOrders; track order.id) {
                <tr>
                  <td><span class="fw-medium">#{{ order.orderNumber }}</span></td>
                  <td>
                    @switch (order.type) {
                      @case ('Restaurant') { <span class="badge bg-warning-subtle text-warning"><i class="bi bi-shop me-1"></i>Restaurant</span> }
                      @case ('Service') { <span class="badge bg-info-subtle text-info"><i class="bi bi-tools me-1"></i>Service</span> }
                      @case ('Grocery') { <span class="badge bg-success-subtle text-success"><i class="bi bi-cart3 me-1"></i>Courses</span> }
                    }
                  </td>
                  <td>{{ order.customerName }}</td>
                  <td class="fw-medium">{{ order.amount | number:'1.2-2' }} MAD</td>
                  <td>
                    @let s = (order.status | statusBadge);
                    <span class="badge-status" [class]="'bg-' + s.color + '-subtle text-' + s.color">{{ s.label }}</span>
                  </td>
                  <td class="text-muted">{{ order.createdAt | date:'dd/MM HH:mm' }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    }
  `
})
export class DashboardComponent implements OnInit, AfterViewInit {
  revenueChartRef = viewChild<ElementRef>('revenueChart');
  ordersChartRef = viewChild<ElementRef>('ordersChart');

  loading = signal(true);
  stats = signal<DashboardStats>({
    totalRevenue: 0, revenueChange: 0, totalOrders: 0, ordersChange: 0,
    activeClients: 0, clientsChange: 0, activeProviders: 0, providersChange: 0,
    restaurantOrders: 0, serviceInterventions: 0, groceryOrders: 0, activeDeliverers: 0,
    revenueChart: [], ordersChart: [], topRestaurants: [], topProviders: [], recentOrders: []
  });

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadData('week');
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.initCharts(), 500);
  }

  onPeriodChange(event: Event): void {
    const period = (event.target as HTMLSelectElement).value;
    this.loadData(period);
  }

  loadData(period: string): void {
    this.loading.set(true);
    this.api.get<DashboardStats>('admin/dashboard', { period }).subscribe({
      next: res => {
        if (res.success) {
          this.stats.set(res.data);
          setTimeout(() => this.initCharts(), 100);
        }
        this.loading.set(false);
      },
      error: () => {
        // Demo data for development
        this.stats.set({
          totalRevenue: 2847500, revenueChange: 12.5, totalOrders: 15420, ordersChange: 8.3,
          activeClients: 45200, clientsChange: 15.2, activeProviders: 1250, providersChange: 6.1,
          restaurantOrders: 8500, serviceInterventions: 3200, groceryOrders: 3720, activeDeliverers: 340,
          revenueChart: [
            { label: 'Lun', value: 380000 }, { label: 'Mar', value: 420000 }, { label: 'Mer', value: 350000 },
            { label: 'Jeu', value: 510000 }, { label: 'Ven', value: 620000 }, { label: 'Sam', value: 450000 },
            { label: 'Dim', value: 320000 }
          ],
          ordersChart: [
            { label: 'Restaurants', value: 8500 }, { label: 'Services', value: 3200 }, { label: 'Courses', value: 3720 }
          ],
          topRestaurants: [], topProviders: [],
          recentOrders: [
            { id: '1', orderNumber: 'CMD-8854', type: 'Restaurant', customerName: 'Ahmed B.', amount: 185.50, status: 'Delivered', createdAt: new Date().toISOString() },
            { id: '2', orderNumber: 'SRV-2241', type: 'Service', customerName: 'Fatima Z.', amount: 450.00, status: 'InProgress', createdAt: new Date().toISOString() },
            { id: '3', orderNumber: 'GRC-1102', type: 'Grocery', customerName: 'Youssef M.', amount: 324.75, status: 'Preparing', createdAt: new Date().toISOString() },
            { id: '4', orderNumber: 'CMD-8853', type: 'Restaurant', customerName: 'Sara L.', amount: 98.00, status: 'InTransit', createdAt: new Date().toISOString() },
            { id: '5', orderNumber: 'SRV-2240', type: 'Service', customerName: 'Omar K.', amount: 800.00, status: 'Confirmed', createdAt: new Date().toISOString() },
          ]
        });
        setTimeout(() => this.initCharts(), 100);
        this.loading.set(false);
      }
    });
  }

  private initCharts(): void {
    const rc = this.revenueChartRef()?.nativeElement;
    const oc = this.ordersChartRef()?.nativeElement;
    if (!rc || !oc) return;

    const data = this.stats();
    Chart.getChart(rc)?.destroy();
    Chart.getChart(oc)?.destroy();

    new Chart(rc, {
      type: 'bar',
      data: {
        labels: data.revenueChart.map(d => d.label),
        datasets: [{
          label: 'CA (MAD)',
          data: data.revenueChart.map(d => d.value),
          backgroundColor: 'rgba(79, 70, 229, 0.2)',
          borderColor: '#4f46e5',
          borderWidth: 2,
          borderRadius: 8,
          barThickness: 32
        }]
      },
      options: {
        responsive: true,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true, ticks: { callback: v => (Number(v) / 1000) + 'K' } } }
      }
    });

    new Chart(oc, {
      type: 'doughnut',
      data: {
        labels: data.ordersChart.map(d => d.label),
        datasets: [{
          data: data.ordersChart.map(d => d.value),
          backgroundColor: ['#f59e0b', '#0ea5e9', '#10b981'],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        cutout: '65%',
        plugins: { legend: { position: 'bottom' } }
      }
    });
  }
}
TSEOF

# ============================================================
# USERS
# ============================================================
mkdir -p "$BASE/users"

cat > "$BASE/users/users.component.ts" << 'TSEOF'
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { UserDto, PaginatedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Gestion des Utilisateurs</h4>
        <p class="text-muted mb-0">{{ totalCount() }} utilisateurs inscrits</p>
      </div>
      <button class="btn btn-primary" (click)="exportUsers()">
        <i class="bi bi-download me-2"></i>Exporter
      </button>
    </div>

    <!-- Filters -->
    <div class="table-card mb-4">
      <div class="card-body p-3">
        <div class="row g-3 align-items-end">
          <div class="col-md-4">
            <label class="form-label small fw-medium">Recherche</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-search"></i></span>
              <input type="text" class="form-control" placeholder="Nom, email, téléphone..." [(ngModel)]="search" (input)="onSearch()" />
            </div>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Rôle</label>
            <select class="form-select" [(ngModel)]="roleFilter" (change)="loadUsers()">
              <option value="">Tous</option>
              <option value="Client">Client</option>
              <option value="RestaurantOwner">Restaurant</option>
              <option value="ServiceProvider">Prestataire</option>
              <option value="GroceryManager">Magasin</option>
              <option value="Deliverer">Livreur</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Statut</label>
            <select class="form-select" [(ngModel)]="statusFilter" (change)="loadUsers()">
              <option value="">Tous</option>
              <option value="true">Actif</option>
              <option value="false">Inactif</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Tri</label>
            <select class="form-select" [(ngModel)]="sortBy" (change)="loadUsers()">
              <option value="createdAt">Date inscription</option>
              <option value="lastLoginAt">Dernière connexion</option>
              <option value="totalSpent">Total dépensé</option>
            </select>
          </div>
          <div class="col-md-2">
            <button class="btn btn-outline-secondary w-100" (click)="resetFilters()">
              <i class="bi bi-x-circle me-1"></i>Réinitialiser
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Table -->
    @if (loading()) {
      <app-loading />
    } @else if (users().length === 0) {
      <app-empty-state icon="bi-people" message="Aucun utilisateur trouvé" />
    } @else {
      <div class="table-card">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Utilisateur</th>
                <th>Rôle</th>
                <th>Téléphone</th>
                <th>Statut</th>
                <th>Commandes</th>
                <th>Dépensé</th>
                <th>Inscrit le</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (user of users(); track user.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="rounded-circle bg-primary bg-opacity-10 text-primary d-flex align-items-center justify-content-center" style="width:36px;height:36px;font-size:0.8rem;font-weight:600">
                        {{ user.firstName[0] }}{{ user.lastName[0] }}
                      </div>
                      <div>
                        <div class="fw-medium">{{ user.firstName }} {{ user.lastName }}</div>
                        <small class="text-muted">{{ user.email }}</small>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span class="badge bg-primary-subtle text-primary">{{ user.role }}</span>
                  </td>
                  <td>{{ user.phoneNumber || '-' }}</td>
                  <td>
                    @if (user.isActive) {
                      <span class="badge-status bg-success-subtle text-success">Actif</span>
                    } @else {
                      <span class="badge-status bg-danger-subtle text-danger">Inactif</span>
                    }
                  </td>
                  <td>{{ user.totalOrders }}</td>
                  <td>{{ user.totalSpent | number:'1.0-0' }} MAD</td>
                  <td class="text-muted">{{ user.createdAt | date:'dd/MM/yyyy' }}</td>
                  <td class="text-end">
                    <div class="dropdown">
                      <button class="btn btn-sm btn-light" data-bs-toggle="dropdown">
                        <i class="bi bi-three-dots-vertical"></i>
                      </button>
                      <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#"><i class="bi bi-eye me-2"></i>Voir profil</a></li>
                        <li>
                          @if (user.isActive) {
                            <a class="dropdown-item text-warning" (click)="toggleUser(user)" style="cursor:pointer">
                              <i class="bi bi-pause-circle me-2"></i>Suspendre
                            </a>
                          } @else {
                            <a class="dropdown-item text-success" (click)="toggleUser(user)" style="cursor:pointer">
                              <i class="bi bi-play-circle me-2"></i>Activer
                            </a>
                          }
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item text-danger" (click)="prepareDelete(user)" style="cursor:pointer"><i class="bi bi-trash me-2"></i>Supprimer</a></li>
                      </ul>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
        <div class="p-3 border-top">
          <app-pagination [currentPage]="page()" [totalPages]="totalPages()" (pageChange)="goToPage($event)" />
        </div>
      </div>
    }

    <app-confirm-modal [show]="showDeleteModal()" title="Supprimer l'utilisateur"
      [message]="'Êtes-vous sûr de vouloir supprimer ' + (selectedUser()?.firstName || '') + ' ' + (selectedUser()?.lastName || '') + ' ?'"
      confirmText="Supprimer" (confirmed)="deleteUser()" (cancelled)="showDeleteModal.set(false)" />
  `
})
export class UsersComponent implements OnInit {
  users = signal<UserDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  search = '';
  roleFilter = '';
  statusFilter = '';
  sortBy = 'createdAt';

  showDeleteModal = signal(false);
  selectedUser = signal<UserDto | null>(null);

  private searchTimeout: any;

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => this.loadUsers(), 400);
  }

  loadUsers(): void {
    this.loading.set(true);
    this.api.getPaginated<UserDto>('admin/users', {
      search: this.search, role: this.roleFilter, isActive: this.statusFilter,
      sortBy: this.sortBy, page: this.page(), pageSize: 15
    }).subscribe({
      next: res => {
        if (res.success) {
          this.users.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.totalCount.set(res.data.totalCount);
        }
        this.loading.set(false);
      },
      error: () => {
        // Demo
        this.users.set([
          { id: '1', firstName: 'Ahmed', lastName: 'Bennani', email: 'ahmed@mail.com', phoneNumber: '+212612345678', photoUrl: '', role: 'Client', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-01-15', lastLoginAt: '2025-06-01', totalOrders: 45, totalSpent: 12500 },
          { id: '2', firstName: 'Fatima', lastName: 'Zahra', email: 'fatima@mail.com', phoneNumber: '+212698765432', photoUrl: '', role: 'Client', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-02-20', lastLoginAt: '2025-06-02', totalOrders: 32, totalSpent: 8900 },
          { id: '3', firstName: 'Youssef', lastName: 'Alami', email: 'youssef@restaurant.ma', phoneNumber: '+212655555555', photoUrl: '', role: 'RestaurantOwner', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-01-10', lastLoginAt: '2025-06-02', totalOrders: 0, totalSpent: 0 },
          { id: '4', firstName: 'Omar', lastName: 'Tazi', email: 'omar@services.ma', phoneNumber: '+212644444444', photoUrl: '', role: 'ServiceProvider', isActive: false, emailVerified: true, phoneVerified: false, createdAt: '2025-03-05', lastLoginAt: '2025-05-20', totalOrders: 0, totalSpent: 0 },
        ]);
        this.totalCount.set(4);
        this.totalPages.set(1);
        this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadUsers(); }
  resetFilters(): void { this.search = ''; this.roleFilter = ''; this.statusFilter = ''; this.sortBy = 'createdAt'; this.loadUsers(); }
  exportUsers(): void { this.toast.info('Export en cours...'); }

  toggleUser(user: UserDto): void {
    this.api.patch(`admin/users/${user.id}/toggle`, {}).subscribe({
      next: () => { this.toast.success('Statut mis à jour'); this.loadUsers(); },
      error: () => this.toast.error('Erreur lors de la mise à jour')
    });
  }

  prepareDelete(user: UserDto): void {
    this.selectedUser.set(user);
    this.showDeleteModal.set(true);
  }

  deleteUser(): void {
    const user = this.selectedUser();
    if (!user) return;
    this.api.delete(`admin/users/${user.id}`).subscribe({
      next: () => { this.toast.success('Utilisateur supprimé'); this.showDeleteModal.set(false); this.loadUsers(); },
      error: () => this.toast.error('Erreur lors de la suppression')
    });
  }
}
TSEOF

echo "✅ Auth + Dashboard + Users generated!"
