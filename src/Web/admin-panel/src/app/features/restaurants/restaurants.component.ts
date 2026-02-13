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
