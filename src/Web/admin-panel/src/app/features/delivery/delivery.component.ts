import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { DelivererDto } from '../../core/models/api.models';

@Component({
  selector: 'app-delivery',
  standalone: true,
  imports: [FormsModule, DecimalPipe, PaginationComponent, LoadingSpinnerComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Gestion des Livreurs</h4>
        <p class="text-muted mb-0">{{ deliverers().length }} livreurs enregistrés</p>
      </div>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card">
          <div class="stat-icon mb-2" style="background:#ecfdf5"><i class="bi bi-circle-fill" style="color:#10b981;font-size:0.7rem"></i></div>
          <h4 class="fw-bold mb-0">{{ onlineCount() }}</h4>
          <small class="text-muted">En ligne</small>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="stat-icon mb-2" style="background:#fef2f2"><i class="bi bi-circle-fill" style="color:#ef4444;font-size:0.7rem"></i></div>
          <h4 class="fw-bold mb-0">{{ offlineCount() }}</h4>
          <small class="text-muted">Hors ligne</small>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="stat-icon mb-2" style="background:#fef3c7"><i class="bi bi-clock" style="color:#f59e0b"></i></div>
          <h4 class="fw-bold mb-0">{{ pendingCount() }}</h4>
          <small class="text-muted">En attente vérification</small>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <div class="stat-icon mb-2" style="background:#eef2ff"><i class="bi bi-truck" style="color:#4f46e5"></i></div>
          <h4 class="fw-bold mb-0">{{ totalDeliveries() | number }}</h4>
          <small class="text-muted">Livraisons totales</small>
        </div>
      </div>
    </div>

    <div class="table-card">
      <div class="card-header">
        <div class="d-flex gap-2">
          <div class="input-group" style="max-width:260px">
            <span class="input-group-text"><i class="bi bi-search"></i></span>
            <input type="text" class="form-control" placeholder="Rechercher..." [(ngModel)]="search" />
          </div>
          <select class="form-select" style="width:auto" [(ngModel)]="statusFilter">
            <option value="">Tous</option>
            <option value="online">En ligne</option>
            <option value="offline">Hors ligne</option>
            <option value="pending">En attente</option>
          </select>
          <select class="form-select" style="width:auto" [(ngModel)]="vehicleFilter">
            <option value="">Tous véhicules</option>
            <option value="Moto">Moto</option>
            <option value="Voiture">Voiture</option>
            <option value="Velo">Vélo</option>
            <option value="Scooter">Scooter</option>
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
                <th>Livreur</th>
                <th>Véhicule</th>
                <th>Statut</th>
                <th>Note</th>
                <th>Livraisons</th>
                <th>Gains totaux</th>
                <th>Vérification</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (d of filteredDeliverers(); track d.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="position-relative">
                        <div class="rounded-circle bg-info bg-opacity-10 text-info d-flex align-items-center justify-content-center" style="width:40px;height:40px;font-weight:600">
                          {{ d.firstName[0] }}{{ d.lastName[0] }}
                        </div>
                        @if (d.isOnline) {
                          <span class="position-absolute bottom-0 end-0 bg-success rounded-circle border border-2 border-white" style="width:12px;height:12px"></span>
                        }
                      </div>
                      <div>
                        <div class="fw-medium">{{ d.firstName }} {{ d.lastName }}</div>
                        <small class="text-muted">{{ d.phone }}</small>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span class="badge bg-light text-dark">
                      @switch (d.vehicleType) {
                        @case ('Moto') { 🏍️ }
                        @case ('Voiture') { 🚗 }
                        @case ('Velo') { 🚲 }
                        @case ('Scooter') { 🛵 }
                      }
                      {{ d.vehicleType }}
                    </span>
                  </td>
                  <td>
                    @if (d.isOnline) {
                      <span class="badge-status bg-success-subtle text-success">En ligne</span>
                    } @else {
                      <span class="badge-status bg-secondary-subtle text-secondary">Hors ligne</span>
                    }
                  </td>
                  <td><span class="text-warning">★</span> {{ d.rating.toFixed(1) }}</td>
                  <td>{{ d.totalDeliveries | number }}</td>
                  <td class="fw-medium">{{ d.totalEarnings | number:'1.0-0' }} MAD</td>
                  <td>
                    @if (d.isVerified) {
                      <span class="badge bg-success-subtle text-success"><i class="bi bi-check-circle me-1"></i>Vérifié</span>
                    } @else {
                      <span class="badge bg-warning-subtle text-warning"><i class="bi bi-clock me-1"></i>En attente</span>
                    }
                  </td>
                  <td class="text-end">
                    <div class="btn-group btn-group-sm">
                      <button class="btn btn-outline-primary"><i class="bi bi-eye"></i></button>
                      @if (!d.isVerified) {
                        <button class="btn btn-outline-success" (click)="approve(d.id)"><i class="bi bi-check-lg"></i></button>
                      }
                      <button class="btn btn-outline-secondary"><i class="bi bi-geo-alt"></i></button>
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
export class DeliveryComponent implements OnInit {
  deliverers = signal<DelivererDto[]>([]);
  loading = signal(true);
  search = ''; statusFilter = ''; vehicleFilter = '';
  onlineCount = signal(0); offlineCount = signal(0); pendingCount = signal(0); totalDeliveries = signal(0);

  constructor(private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadDeliverers(); }

  filteredDeliverers(): DelivererDto[] {
    let result = this.deliverers();
    if (this.search) result = result.filter(d => `${d.firstName} ${d.lastName}`.toLowerCase().includes(this.search.toLowerCase()));
    if (this.statusFilter === 'online') result = result.filter(d => d.isOnline);
    if (this.statusFilter === 'offline') result = result.filter(d => !d.isOnline && d.isVerified);
    if (this.statusFilter === 'pending') result = result.filter(d => !d.isVerified);
    if (this.vehicleFilter) result = result.filter(d => d.vehicleType === this.vehicleFilter);
    return result;
  }

  loadDeliverers(): void {
    this.loading.set(true);
    this.api.get<DelivererDto[]>('admin/deliverers').subscribe({
      next: res => { if (res.success) this.deliverers.set(res.data); this.loading.set(false); },
      error: () => {
        const data: DelivererDto[] = [
          { id: '1', firstName: 'Karim', lastName: 'Lahlou', phone: '+212611111111', email: '', photoUrl: '', vehicleType: 'Moto', isActive: true, isOnline: true, isVerified: true, rating: 4.8, totalDeliveries: 1250, totalEarnings: 85000, latitude: 33.5731, longitude: -7.5898, createdAt: '2025-01-15' },
          { id: '2', firstName: 'Ali', lastName: 'Tahiri', phone: '+212622222222', email: '', photoUrl: '', vehicleType: 'Voiture', isActive: true, isOnline: true, isVerified: true, rating: 4.5, totalDeliveries: 890, totalEarnings: 62000, latitude: 33.5890, longitude: -7.6100, createdAt: '2025-02-01' },
          { id: '3', firstName: 'Hamid', lastName: 'Rami', phone: '+212633333333', email: '', photoUrl: '', vehicleType: 'Scooter', isActive: true, isOnline: false, isVerified: true, rating: 4.2, totalDeliveries: 450, totalEarnings: 31000, latitude: 0, longitude: 0, createdAt: '2025-03-10' },
          { id: '4', firstName: 'Samir', lastName: 'Bousfiha', phone: '+212644444444', email: '', photoUrl: '', vehicleType: 'Moto', isActive: true, isOnline: false, isVerified: false, rating: 0, totalDeliveries: 0, totalEarnings: 0, latitude: 0, longitude: 0, createdAt: '2025-06-01' },
        ];
        this.deliverers.set(data);
        this.onlineCount.set(data.filter(d => d.isOnline).length);
        this.offlineCount.set(data.filter(d => !d.isOnline && d.isVerified).length);
        this.pendingCount.set(data.filter(d => !d.isVerified).length);
        this.totalDeliveries.set(data.reduce((sum, d) => sum + d.totalDeliveries, 0));
        this.loading.set(false);
      }
    });
  }

  approve(id: string): void { this.api.post(`admin/deliverers/${id}/approve`, {}).subscribe({ next: () => { this.toast.success('Livreur approuvé'); this.loadDeliverers(); } }); }
}
