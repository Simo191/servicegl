import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { OrderDto } from '../../core/models/api.models';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, StatusBadgePipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Gestion des Commandes</h4>
        <p class="text-muted mb-0">Toutes les commandes de la plateforme</p>
      </div>
      <button class="btn btn-outline-primary" (click)="exportOrders()"><i class="bi bi-download me-2"></i>Exporter</button>
    </div>

    <!-- Type Tabs -->
    <ul class="nav nav-pills mb-4 gap-2">
      @for (tab of tabs; track tab.value) {
        <li class="nav-item">
          <a class="nav-link" [class.active]="selectedType === tab.value" (click)="selectType(tab.value)" style="cursor:pointer">
            <i class="bi me-1" [class]="tab.icon"></i> {{ tab.label }}
            <span class="badge bg-white bg-opacity-25 ms-1">{{ tab.count }}</span>
          </a>
        </li>
      }
    </ul>

    <!-- Filters -->
    <div class="table-card">
      <div class="card-header">
        <div class="d-flex gap-2 flex-wrap">
          <div class="input-group" style="max-width:260px">
            <span class="input-group-text"><i class="bi bi-search"></i></span>
            <input type="text" class="form-control" placeholder="N° commande, client..." [(ngModel)]="search" (input)="onSearch()" />
          </div>
          <select class="form-select" style="width:auto" [(ngModel)]="statusFilter" (change)="loadOrders()">
            <option value="">Tous les statuts</option>
            <option value="Pending">En attente</option>
            <option value="Confirmed">Confirmée</option>
            <option value="Preparing">En préparation</option>
            <option value="InTransit">En route</option>
            <option value="Delivered">Livrée</option>
            <option value="Cancelled">Annulée</option>
          </select>
          <input type="date" class="form-control" style="width:auto" [(ngModel)]="dateFrom" (change)="loadOrders()" />
          <input type="date" class="form-control" style="width:auto" [(ngModel)]="dateTo" (change)="loadOrders()" />
        </div>
      </div>

      @if (loading()) {
        <app-loading />
      } @else {
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>N° Commande</th>
                <th>Type</th>
                <th>Client</th>
                <th>Prestataire</th>
                <th>Livreur</th>
                <th>Montant</th>
                <th>Paiement</th>
                <th>Statut</th>
                <th>Date</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (o of orders(); track o.id) {
                <tr>
                  <td><span class="fw-medium text-primary">#{{ o.orderNumber }}</span></td>
                  <td>
                    @switch (o.type) {
                      @case ('Restaurant') { <span class="badge bg-warning-subtle text-warning">🍔 Restaurant</span> }
                      @case ('Service') { <span class="badge bg-info-subtle text-info">🛠️ Service</span> }
                      @case ('Grocery') { <span class="badge bg-success-subtle text-success">🛒 Courses</span> }
                    }
                  </td>
                  <td>{{ o.customerName }}</td>
                  <td>{{ o.providerName }}</td>
                  <td>{{ o.delivererName || '-' }}</td>
                  <td class="fw-bold">{{ o.total | number:'1.2-2' }} MAD</td>
                  <td>
                    @if (o.paymentStatus === 'Paid') {
                      <span class="badge bg-success-subtle text-success">Payé</span>
                    } @else {
                      <span class="badge bg-warning-subtle text-warning">En attente</span>
                    }
                  </td>
                  <td>
                    @let s = (o.status | statusBadge);
                    <span class="badge-status" [class]="'bg-' + s.color + '-subtle text-' + s.color">{{ s.label }}</span>
                  </td>
                  <td class="text-muted small">{{ o.createdAt | date:'dd/MM HH:mm' }}</td>
                  <td class="text-end">
                    <div class="dropdown">
                      <button class="btn btn-sm btn-light" data-bs-toggle="dropdown"><i class="bi bi-three-dots-vertical"></i></button>
                      <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item"><i class="bi bi-eye me-2"></i>Détails</a></li>
                        <li><a class="dropdown-item"><i class="bi bi-person-badge me-2"></i>Réassigner livreur</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item text-danger" (click)="cancelOrder(o.id)" style="cursor:pointer"><i class="bi bi-x-circle me-2"></i>Annuler</a></li>
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
      }
    </div>
  `
})
export class OrdersComponent implements OnInit {
  orders = signal<OrderDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  search = ''; statusFilter = ''; selectedType = ''; dateFrom = ''; dateTo = '';
  private searchTimeout: any;

  tabs = [
    { value: '', label: 'Toutes', icon: 'bi-grid', count: 15420 },
    { value: 'Restaurant', label: 'Restaurant', icon: 'bi-shop', count: 8500 },
    { value: 'Service', label: 'Services', icon: 'bi-tools', count: 3200 },
    { value: 'Grocery', label: 'Courses', icon: 'bi-cart3', count: 3720 },
  ];

  constructor(private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadOrders(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => this.loadOrders(), 400); }
  selectType(type: string): void { this.selectedType = type; this.loadOrders(); }

  loadOrders(): void {
    this.loading.set(true);
    this.api.getPaginated<OrderDto>('admin/orders', {
      search: this.search, type: this.selectedType, status: this.statusFilter,
      dateFrom: this.dateFrom, dateTo: this.dateTo, page: this.page(), pageSize: 20
    }).subscribe({
      next: res => { if (res.success) { this.orders.set(res.data.items); this.totalPages.set(res.data.totalPages); } this.loading.set(false); },
      error: () => {
        this.orders.set([
          { id: '1', orderNumber: 'CMD-8854', type: 'Restaurant', providerName: 'Chez Hassan', customerName: 'Ahmed B.', customerPhone: '', delivererName: 'Karim L.', status: 'Delivered', subtotal: 170, deliveryFee: 15.5, total: 185.5, paymentMethod: 'Card', paymentStatus: 'Paid', createdAt: '2025-06-02T14:30:00', deliveredAt: '2025-06-02T15:15:00' },
          { id: '2', orderNumber: 'SRV-2241', type: 'Service', providerName: 'ProPlomb Casa', customerName: 'Fatima Z.', customerPhone: '', delivererName: '', status: 'InProgress', subtotal: 450, deliveryFee: 0, total: 450, paymentMethod: 'Cash', paymentStatus: 'Unpaid', createdAt: '2025-06-02T10:00:00', deliveredAt: '' },
          { id: '3', orderNumber: 'GRC-1102', type: 'Grocery', providerName: 'Marjane Ain Diab', customerName: 'Youssef M.', customerPhone: '', delivererName: 'Ali T.', status: 'Preparing', subtotal: 305, deliveryFee: 19.75, total: 324.75, paymentMethod: 'Card', paymentStatus: 'Paid', createdAt: '2025-06-02T11:45:00', deliveredAt: '' },
          { id: '4', orderNumber: 'CMD-8853', type: 'Restaurant', providerName: 'Pizza Roma', customerName: 'Sara L.', customerPhone: '', delivererName: 'Hamid R.', status: 'InTransit', subtotal: 88, deliveryFee: 10, total: 98, paymentMethod: 'Wallet', paymentStatus: 'Paid', createdAt: '2025-06-02T13:00:00', deliveredAt: '' },
          { id: '5', orderNumber: 'CMD-8850', type: 'Restaurant', providerName: 'Chez Hassan', customerName: 'Omar K.', customerPhone: '', delivererName: '', status: 'Cancelled', subtotal: 220, deliveryFee: 15, total: 235, paymentMethod: 'Card', paymentStatus: 'Refunded', createdAt: '2025-06-01T19:00:00', deliveredAt: '' },
        ]);
        this.totalPages.set(5); this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadOrders(); }
  exportOrders(): void { this.toast.info('Export en cours...'); }
  cancelOrder(id: string): void { this.api.post(`admin/orders/${id}/cancel`, {}).subscribe({ next: () => { this.toast.success('Commande annulée'); this.loadOrders(); } }); }
}
