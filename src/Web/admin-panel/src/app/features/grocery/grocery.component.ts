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
