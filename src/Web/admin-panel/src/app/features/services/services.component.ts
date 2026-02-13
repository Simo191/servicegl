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
