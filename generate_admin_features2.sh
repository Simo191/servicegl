#!/bin/bash
set -e
BASE="/home/claude/MultiServicesApp/src/Web/admin-panel/src/app/features"

# ============================================================
# RESTAURANTS
# ============================================================
mkdir -p "$BASE/restaurants"

cat > "$BASE/restaurants/restaurants.component.ts" << 'TSEOF'
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { RestaurantDto, PaginatedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-restaurants',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, ConfirmModalComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Gestion des Restaurants</h4>
        <p class="text-muted mb-0">{{ totalCount() }} restaurants enregistrés</p>
      </div>
    </div>

    <!-- Stats -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card">
          <div class="d-flex align-items-center gap-3">
            <div class="stat-icon" style="background:#fef3c7"><i class="bi bi-shop" style="color:#f59e0b"></i></div>
            <div><p class="stat-label mb-0">Total</p><h5 class="fw-bold mb-0">{{ totalCount() }}</h5></div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="d-flex align-items-center gap-3">
            <div class="stat-icon" style="background:#ecfdf5"><i class="bi bi-check-circle" style="color:#10b981"></i></div>
            <div><p class="stat-label mb-0">Vérifiés</p><h5 class="fw-bold mb-0">{{ verifiedCount() }}</h5></div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="d-flex align-items-center gap-3">
            <div class="stat-icon" style="background:#fef2f2"><i class="bi bi-clock" style="color:#ef4444"></i></div>
            <div><p class="stat-label mb-0">En attente</p><h5 class="fw-bold mb-0">{{ pendingCount() }}</h5></div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="d-flex align-items-center gap-3">
            <div class="stat-icon" style="background:#eef2ff"><i class="bi bi-star" style="color:#4f46e5"></i></div>
            <div><p class="stat-label mb-0">Note moyenne</p><h5 class="fw-bold mb-0">4.3 ★</h5></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters + Table -->
    <div class="table-card">
      <div class="card-header">
        <div class="d-flex gap-2 flex-wrap">
          <div class="input-group" style="max-width:280px">
            <span class="input-group-text"><i class="bi bi-search"></i></span>
            <input type="text" class="form-control" placeholder="Rechercher..." [(ngModel)]="search" (input)="onSearch()" />
          </div>
          <select class="form-select" style="width:auto" [(ngModel)]="statusFilter" (change)="loadRestaurants()">
            <option value="">Tous les statuts</option>
            <option value="verified">Vérifiés</option>
            <option value="pending">En attente</option>
            <option value="suspended">Suspendus</option>
          </select>
          <select class="form-select" style="width:auto" [(ngModel)]="cuisineFilter" (change)="loadRestaurants()">
            <option value="">Toutes cuisines</option>
            <option value="Marocain">Marocain</option>
            <option value="Italien">Italien</option>
            <option value="Asiatique">Asiatique</option>
            <option value="Burger">Burger</option>
            <option value="Pizza">Pizza</option>
            <option value="Sushi">Sushi</option>
          </select>
        </div>
      </div>

      @if (loading()) {
        <app-loading />
      } @else {
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Restaurant</th>
                <th>Cuisine</th>
                <th>Ville</th>
                <th>Note</th>
                <th>Commandes</th>
                <th>CA</th>
                <th>Commission</th>
                <th>Statut</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (r of restaurants(); track r.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="rounded bg-warning bg-opacity-10 d-flex align-items-center justify-content-center" style="width:40px;height:40px">
                        <i class="bi bi-shop text-warning"></i>
                      </div>
                      <div>
                        <div class="fw-medium">{{ r.name }}</div>
                        <small class="text-muted">{{ r.ownerName }}</small>
                      </div>
                    </div>
                  </td>
                  <td><span class="badge bg-light text-dark">{{ r.cuisineType }}</span></td>
                  <td>{{ r.city }}</td>
                  <td><span class="text-warning">★</span> {{ r.rating.toFixed(1) }}</td>
                  <td>{{ r.totalOrders | number }}</td>
                  <td class="fw-medium">{{ r.totalRevenue | number:'1.0-0' }} MAD</td>
                  <td>{{ r.commissionRate }}%</td>
                  <td>
                    @if (r.isVerified && r.isActive) {
                      <span class="badge-status bg-success-subtle text-success">Actif</span>
                    } @else if (!r.isVerified) {
                      <span class="badge-status bg-warning-subtle text-warning">En attente</span>
                    } @else {
                      <span class="badge-status bg-danger-subtle text-danger">Suspendu</span>
                    }
                  </td>
                  <td class="text-end">
                    <div class="btn-group btn-group-sm">
                      <button class="btn btn-outline-primary" title="Voir"><i class="bi bi-eye"></i></button>
                      @if (!r.isVerified) {
                        <button class="btn btn-outline-success" title="Approuver" (click)="approve(r.id)"><i class="bi bi-check-lg"></i></button>
                      }
                      <button class="btn btn-outline-danger" title="Suspendre" (click)="toggleStatus(r)">
                        <i class="bi" [class.bi-pause]="r.isActive" [class.bi-play]="!r.isActive"></i>
                      </button>
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
      }
    </div>
  `
})
export class RestaurantsComponent implements OnInit {
  restaurants = signal<RestaurantDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  verifiedCount = signal(0);
  pendingCount = signal(0);
  search = '';
  statusFilter = '';
  cuisineFilter = '';
  private searchTimeout: any;

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void { this.loadRestaurants(); }

  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => this.loadRestaurants(), 400); }

  loadRestaurants(): void {
    this.loading.set(true);
    this.api.getPaginated<RestaurantDto>('admin/restaurants', {
      search: this.search, status: this.statusFilter, cuisine: this.cuisineFilter, page: this.page(), pageSize: 15
    }).subscribe({
      next: res => { if (res.success) { this.restaurants.set(res.data.items); this.totalPages.set(res.data.totalPages); this.totalCount.set(res.data.totalCount); } this.loading.set(false); },
      error: () => {
        this.restaurants.set([
          { id: '1', name: 'Chez Hassan', slug: 'chez-hassan', description: '', ownerName: 'Hassan M.', cuisineType: 'Marocain', address: '12 Rue Mohammed V', city: 'Casablanca', phone: '+212600000001', email: 'hassan@mail.com', logoUrl: '', coverUrl: '', isVerified: true, isActive: true, rating: 4.5, totalOrders: 1250, totalRevenue: 450000, commissionRate: 15, createdAt: '2025-01-10' },
          { id: '2', name: 'Pizza Roma', slug: 'pizza-roma', description: '', ownerName: 'Marco L.', cuisineType: 'Italien', address: '8 Bd Anfa', city: 'Casablanca', phone: '+212600000002', email: 'marco@mail.com', logoUrl: '', coverUrl: '', isVerified: true, isActive: true, rating: 4.2, totalOrders: 890, totalRevenue: 320000, commissionRate: 15, createdAt: '2025-02-15' },
          { id: '3', name: 'Sushi Master', slug: 'sushi-master', description: '', ownerName: 'Kenji T.', cuisineType: 'Sushi', address: '22 Av Hassan II', city: 'Rabat', phone: '+212600000003', email: 'kenji@mail.com', logoUrl: '', coverUrl: '', isVerified: false, isActive: true, rating: 0, totalOrders: 0, totalRevenue: 0, commissionRate: 15, createdAt: '2025-06-01' },
        ]);
        this.totalCount.set(3); this.verifiedCount.set(2); this.pendingCount.set(1); this.totalPages.set(1); this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadRestaurants(); }
  approve(id: string): void { this.api.post(`admin/restaurants/${id}/approve`, {}).subscribe({ next: () => { this.toast.success('Restaurant approuvé'); this.loadRestaurants(); } }); }
  toggleStatus(r: RestaurantDto): void { this.api.patch(`admin/restaurants/${r.id}/toggle`, {}).subscribe({ next: () => { this.toast.success('Statut mis à jour'); this.loadRestaurants(); } }); }
}
TSEOF

# ============================================================
# SERVICES
# ============================================================
mkdir -p "$BASE/services"

cat > "$BASE/services/services.component.ts" << 'TSEOF'
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { ServiceProviderDto } from '../../core/models/api.models';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Prestataires de Services</h4>
        <p class="text-muted mb-0">{{ totalCount() }} prestataires enregistrés</p>
      </div>
    </div>

    <div class="row g-3 mb-4">
      @for (cat of categories; track cat.name) {
        <div class="col-md-3 col-6">
          <div class="stat-card text-center">
            <i class="bi" [class]="cat.icon" style="font-size:1.5rem" [style.color]="cat.color"></i>
            <p class="fw-bold mb-0 mt-1">{{ cat.count }}</p>
            <small class="text-muted">{{ cat.name }}</small>
          </div>
        </div>
      }
    </div>

    <div class="table-card">
      <div class="card-header">
        <div class="d-flex gap-2 flex-wrap">
          <div class="input-group" style="max-width:280px">
            <span class="input-group-text"><i class="bi bi-search"></i></span>
            <input type="text" class="form-control" placeholder="Rechercher..." [(ngModel)]="search" (input)="onSearch()" />
          </div>
          <select class="form-select" style="width:auto" [(ngModel)]="categoryFilter" (change)="loadProviders()">
            <option value="">Toutes catégories</option>
            <option value="Plomberie">Plomberie</option>
            <option value="Electricite">Électricité</option>
            <option value="Menage">Ménage</option>
            <option value="Peinture">Peinture</option>
            <option value="Jardinage">Jardinage</option>
            <option value="Climatisation">Climatisation</option>
            <option value="Demenagement">Déménagement</option>
          </select>
        </div>
      </div>

      @if (loading()) {
        <app-loading />
      } @else {
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Prestataire</th>
                <th>Catégorie</th>
                <th>Ville</th>
                <th>Expérience</th>
                <th>Note</th>
                <th>Interventions</th>
                <th>CA</th>
                <th>Statut</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (p of providers(); track p.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="rounded bg-info bg-opacity-10 d-flex align-items-center justify-content-center" style="width:40px;height:40px">
                        <i class="bi bi-tools text-info"></i>
                      </div>
                      <div>
                        <div class="fw-medium">{{ p.companyName }}</div>
                        <small class="text-muted">{{ p.ownerName }}</small>
                      </div>
                    </div>
                  </td>
                  <td><span class="badge bg-info-subtle text-info">{{ p.category }}</span></td>
                  <td>{{ p.city }}</td>
                  <td>{{ p.yearsExperience }} ans</td>
                  <td><span class="text-warning">★</span> {{ p.rating.toFixed(1) }}</td>
                  <td>{{ p.totalInterventions | number }}</td>
                  <td class="fw-medium">{{ p.totalRevenue | number:'1.0-0' }} MAD</td>
                  <td>
                    @if (p.isVerified && p.isActive) {
                      <span class="badge-status bg-success-subtle text-success">Actif</span>
                    } @else if (!p.isVerified) {
                      <span class="badge-status bg-warning-subtle text-warning">En attente</span>
                    } @else {
                      <span class="badge-status bg-danger-subtle text-danger">Suspendu</span>
                    }
                  </td>
                  <td class="text-end">
                    <div class="btn-group btn-group-sm">
                      <button class="btn btn-outline-primary"><i class="bi bi-eye"></i></button>
                      @if (!p.isVerified) {
                        <button class="btn btn-outline-success" (click)="approve(p.id)"><i class="bi bi-check-lg"></i></button>
                      }
                      <button class="btn btn-outline-danger"><i class="bi bi-pause"></i></button>
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
      }
    </div>
  `
})
export class ServicesComponent implements OnInit {
  providers = signal<ServiceProviderDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  search = '';
  categoryFilter = '';
  private searchTimeout: any;

  categories = [
    { name: 'Plomberie', icon: 'bi-droplet', color: '#0ea5e9', count: 85 },
    { name: 'Électricité', icon: 'bi-lightning', color: '#f59e0b', count: 72 },
    { name: 'Ménage', icon: 'bi-house', color: '#10b981', count: 120 },
    { name: 'Peinture', icon: 'bi-palette', color: '#ec4899', count: 45 },
    { name: 'Jardinage', icon: 'bi-tree', color: '#22c55e', count: 38 },
    { name: 'Climatisation', icon: 'bi-snow', color: '#06b6d4', count: 55 },
    { name: 'Déménagement', icon: 'bi-truck', color: '#8b5cf6', count: 30 },
    { name: 'Réparation', icon: 'bi-wrench', color: '#64748b', count: 60 },
  ];

  constructor(private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadProviders(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => this.loadProviders(), 400); }

  loadProviders(): void {
    this.loading.set(true);
    this.api.getPaginated<ServiceProviderDto>('admin/service-providers', {
      search: this.search, category: this.categoryFilter, page: this.page(), pageSize: 15
    }).subscribe({
      next: res => { if (res.success) { this.providers.set(res.data.items); this.totalPages.set(res.data.totalPages); this.totalCount.set(res.data.totalCount); } this.loading.set(false); },
      error: () => {
        this.providers.set([
          { id: '1', companyName: 'ProPlomb Casa', ownerName: 'Khalid A.', category: 'Plomberie', description: '', address: '', city: 'Casablanca', phone: '', email: '', logoUrl: '', isVerified: true, isActive: true, rating: 4.7, totalInterventions: 320, totalRevenue: 280000, commissionRate: 12, yearsExperience: 15, createdAt: '2025-01-05' },
          { id: '2', companyName: 'ElecPro Maroc', ownerName: 'Nabil B.', category: 'Électricité', description: '', address: '', city: 'Rabat', phone: '', email: '', logoUrl: '', isVerified: true, isActive: true, rating: 4.4, totalInterventions: 210, totalRevenue: 195000, commissionRate: 12, yearsExperience: 10, createdAt: '2025-02-10' },
          { id: '3', companyName: 'Clean House', ownerName: 'Samira R.', category: 'Ménage', description: '', address: '', city: 'Casablanca', phone: '', email: '', logoUrl: '', isVerified: true, isActive: true, rating: 4.8, totalInterventions: 540, totalRevenue: 320000, commissionRate: 12, yearsExperience: 8, createdAt: '2025-01-20' },
        ]);
        this.totalCount.set(3); this.totalPages.set(1); this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadProviders(); }
  approve(id: string): void { this.api.post(`admin/service-providers/${id}/approve`, {}).subscribe({ next: () => { this.toast.success('Prestataire approuvé'); this.loadProviders(); } }); }
}
TSEOF

# ============================================================
# GROCERY
# ============================================================
mkdir -p "$BASE/grocery"

cat > "$BASE/grocery/grocery.component.ts" << 'TSEOF'
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { GroceryStoreDto } from '../../core/models/api.models';

@Component({
  selector: 'app-grocery',
  standalone: true,
  imports: [FormsModule, DecimalPipe, DatePipe, PaginationComponent, LoadingSpinnerComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Magasins / Courses en ligne</h4>
        <p class="text-muted mb-0">{{ stores().length }} magasins partenaires</p>
      </div>
    </div>

    <!-- Store Brand Cards -->
    <div class="row g-3 mb-4">
      @for (brand of brands; track brand.name) {
        <div class="col-md col-6">
          <div class="stat-card text-center" style="cursor:pointer" [class.border-primary]="selectedBrand === brand.name" (click)="filterByBrand(brand.name)">
            <i class="bi bi-building fs-3" [style.color]="brand.color"></i>
            <h6 class="fw-bold mt-2 mb-0">{{ brand.name }}</h6>
            <small class="text-muted">{{ brand.count }} magasins</small>
          </div>
        </div>
      }
    </div>

    <div class="table-card">
      <div class="card-header">
        <h6 class="fw-bold mb-0">Liste des magasins</h6>
        <div class="input-group" style="max-width:250px">
          <span class="input-group-text"><i class="bi bi-search"></i></span>
          <input type="text" class="form-control" placeholder="Rechercher..." [(ngModel)]="search" (input)="onSearch()" />
        </div>
      </div>

      @if (loading()) {
        <app-loading />
      } @else {
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Magasin</th>
                <th>Enseigne</th>
                <th>Ville</th>
                <th>Produits</th>
                <th>Commandes</th>
                <th>CA</th>
                <th>Commission</th>
                <th>Statut</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (s of filteredStores(); track s.id) {
                <tr>
                  <td class="fw-medium">{{ s.name }}</td>
                  <td><span class="badge bg-success-subtle text-success">{{ s.brand }}</span></td>
                  <td>{{ s.city }}</td>
                  <td>{{ s.totalProducts | number }}</td>
                  <td>{{ s.totalOrders | number }}</td>
                  <td class="fw-medium">{{ s.totalRevenue | number:'1.0-0' }} MAD</td>
                  <td>{{ s.commissionRate }}%</td>
                  <td>
                    @if (s.isActive) {
                      <span class="badge-status bg-success-subtle text-success">Actif</span>
                    } @else {
                      <span class="badge-status bg-danger-subtle text-danger">Inactif</span>
                    }
                  </td>
                  <td class="text-end">
                    <div class="btn-group btn-group-sm">
                      <button class="btn btn-outline-primary"><i class="bi bi-eye"></i></button>
                      <button class="btn btn-outline-secondary"><i class="bi bi-pencil"></i></button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `
})
export class GroceryComponent implements OnInit {
  stores = signal<GroceryStoreDto[]>([]);
  loading = signal(true);
  search = '';
  selectedBrand = '';
  private searchTimeout: any;

  brands = [
    { name: 'Marjane', color: '#e11d48', count: 12 },
    { name: 'Carrefour', color: '#2563eb', count: 8 },
    { name: 'Aswak Assalam', color: '#16a34a', count: 6 },
    { name: 'Acima', color: '#ea580c', count: 10 },
    { name: 'Label\'Vie', color: '#7c3aed', count: 5 },
  ];

  constructor(private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadStores(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => {}, 400); }
  filterByBrand(brand: string): void { this.selectedBrand = this.selectedBrand === brand ? '' : brand; }

  filteredStores(): GroceryStoreDto[] {
    let result = this.stores();
    if (this.selectedBrand) result = result.filter(s => s.brand === this.selectedBrand);
    if (this.search) result = result.filter(s => s.name.toLowerCase().includes(this.search.toLowerCase()));
    return result;
  }

  loadStores(): void {
    this.loading.set(true);
    this.api.get<GroceryStoreDto[]>('admin/grocery-stores').subscribe({
      next: res => { if (res.success) this.stores.set(res.data); this.loading.set(false); },
      error: () => {
        this.stores.set([
          { id: '1', name: 'Marjane Ain Diab', brand: 'Marjane', address: 'Ain Diab', city: 'Casablanca', phone: '', isActive: true, totalProducts: 12500, totalOrders: 3400, totalRevenue: 1250000, commissionRate: 8, createdAt: '2025-01-01' },
          { id: '2', name: 'Carrefour Anfa', brand: 'Carrefour', address: 'Anfa Place', city: 'Casablanca', phone: '', isActive: true, totalProducts: 15000, totalOrders: 2800, totalRevenue: 980000, commissionRate: 8, createdAt: '2025-01-01' },
          { id: '3', name: 'Acima Maarif', brand: 'Acima', address: 'Maarif', city: 'Casablanca', phone: '', isActive: true, totalProducts: 8500, totalOrders: 1500, totalRevenue: 450000, commissionRate: 8, createdAt: '2025-01-01' },
          { id: '4', name: 'Aswak Assalam Agdal', brand: 'Aswak Assalam', address: 'Agdal', city: 'Rabat', phone: '', isActive: true, totalProducts: 11000, totalOrders: 2100, totalRevenue: 720000, commissionRate: 8, createdAt: '2025-01-01' },
        ]);
        this.loading.set(false);
      }
    });
  }
}
TSEOF

echo "✅ Restaurants + Services + Grocery generated!"
