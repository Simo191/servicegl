#!/bin/bash
set -e
BASE="/home/claude/MultiServicesApp/src/Web/admin-panel/src/app/features"

# ============================================================
# ORDERS
# ============================================================
mkdir -p "$BASE/orders"

cat > "$BASE/orders/orders.component.ts" << 'TSEOF'
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
TSEOF

# ============================================================
# DELIVERY
# ============================================================
mkdir -p "$BASE/delivery"

cat > "$BASE/delivery/delivery.component.ts" << 'TSEOF'
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
TSEOF

# ============================================================
# FINANCE
# ============================================================
mkdir -p "$BASE/finance"

cat > "$BASE/finance/finance.component.ts" << 'TSEOF'
import { Component, OnInit, signal, viewChild, ElementRef, AfterViewInit } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { FinanceOverview } from '../../core/models/api.models';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-finance',
  standalone: true,
  imports: [DecimalPipe, DatePipe, StatCardComponent, LoadingSpinnerComponent, StatusBadgePipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Finances</h4>
        <p class="text-muted mb-0">Vue d'ensemble financière</p>
      </div>
      <button class="btn btn-outline-primary"><i class="bi bi-download me-2"></i>Export comptable</button>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-xl-3 col-md-6">
        <app-stat-card label="Revenu total" [value]="data().totalRevenue" icon="bi-currency-dollar" bgColor="#ecfdf5" iconColor="#10b981" [isCurrency]="true" />
      </div>
      <div class="col-xl-3 col-md-6">
        <app-stat-card label="Commissions" [value]="data().totalCommissions" icon="bi-percent" bgColor="#eef2ff" iconColor="#4f46e5" [isCurrency]="true" />
      </div>
      <div class="col-xl-3 col-md-6">
        <app-stat-card label="Paiements effectués" [value]="data().totalPayouts" icon="bi-send" bgColor="#fef3c7" iconColor="#f59e0b" [isCurrency]="true" />
      </div>
      <div class="col-xl-3 col-md-6">
        <app-stat-card label="Paiements en attente" [value]="data().pendingPayouts" icon="bi-hourglass-split" bgColor="#fce7f3" iconColor="#ec4899" [isCurrency]="true" />
      </div>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-lg-8">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Revenus mensuels</h6></div>
          <div class="p-3"><canvas #revenueChart height="300"></canvas></div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Revenus par module</h6></div>
          <div class="p-3"><canvas #moduleChart height="300"></canvas></div>
        </div>
      </div>
    </div>

    <div class="table-card">
      <div class="card-header">
        <h6 class="fw-bold mb-0">Transactions récentes</h6>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead>
            <tr><th>Référence</th><th>Type</th><th>Description</th><th>Montant</th><th>Statut</th><th>Date</th></tr>
          </thead>
          <tbody>
            @for (t of data().recentTransactions; track t.id) {
              <tr>
                <td class="fw-medium text-primary">{{ t.reference }}</td>
                <td><span class="badge bg-light text-dark">{{ t.type }}</span></td>
                <td>{{ t.description }}</td>
                <td class="fw-bold" [class.text-success]="t.amount > 0" [class.text-danger]="t.amount < 0">
                  {{ t.amount > 0 ? '+' : '' }}{{ t.amount | number:'1.2-2' }} MAD
                </td>
                <td>
                  @let s = (t.status | statusBadge);
                  <span class="badge-status" [class]="'bg-' + s.color + '-subtle text-' + s.color">{{ s.label }}</span>
                </td>
                <td class="text-muted">{{ t.date | date:'dd/MM/yyyy HH:mm' }}</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class FinanceComponent implements OnInit, AfterViewInit {
  revenueChartRef = viewChild<ElementRef>('revenueChart');
  moduleChartRef = viewChild<ElementRef>('moduleChart');
  data = signal<FinanceOverview>({ totalRevenue: 0, totalCommissions: 0, totalPayouts: 0, pendingPayouts: 0, revenueByModule: [], monthlyRevenue: [], recentTransactions: [] });

  constructor(private api: ApiService) {}
  ngOnInit(): void { this.loadData(); }
  ngAfterViewInit(): void { setTimeout(() => this.initCharts(), 500); }

  loadData(): void {
    this.api.get<FinanceOverview>('admin/finance').subscribe({
      next: res => { if (res.success) { this.data.set(res.data); setTimeout(() => this.initCharts(), 100); } },
      error: () => {
        this.data.set({
          totalRevenue: 8540000, totalCommissions: 1025000, totalPayouts: 7200000, pendingPayouts: 315000,
          revenueByModule: [{ module: 'Restaurants', amount: 4200000 }, { module: 'Services', amount: 2100000 }, { module: 'Courses', amount: 2240000 }],
          monthlyRevenue: [
            { label: 'Jan', value: 1200000 }, { label: 'Fév', value: 1350000 }, { label: 'Mar', value: 1100000 },
            { label: 'Avr', value: 1500000 }, { label: 'Mai', value: 1680000 }, { label: 'Juin', value: 1710000 }
          ],
          recentTransactions: [
            { id: '1', type: 'Commission', description: 'Commission restaurant - Chez Hassan', amount: 27.83, status: 'Completed', date: '2025-06-02T14:30:00', reference: 'TRX-100452' },
            { id: '2', type: 'Paiement', description: 'Virement prestataire - ProPlomb', amount: -450.00, status: 'Processing', date: '2025-06-02T12:00:00', reference: 'TRX-100451' },
            { id: '3', type: 'Remboursement', description: 'Remboursement commande #CMD-8850', amount: -235.00, status: 'Completed', date: '2025-06-01T19:30:00', reference: 'TRX-100450' },
          ]
        });
        setTimeout(() => this.initCharts(), 100);
      }
    });
  }

  private initCharts(): void {
    const rc = this.revenueChartRef()?.nativeElement;
    const mc = this.moduleChartRef()?.nativeElement;
    if (!rc || !mc) return;
    const d = this.data();
    Chart.getChart(rc)?.destroy(); Chart.getChart(mc)?.destroy();

    new Chart(rc, {
      type: 'line',
      data: { labels: d.monthlyRevenue.map(m => m.label), datasets: [{ label: 'CA (MAD)', data: d.monthlyRevenue.map(m => m.value), borderColor: '#4f46e5', backgroundColor: 'rgba(79,70,229,0.1)', fill: true, tension: 0.4, borderWidth: 2 }] },
      options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { callback: v => (Number(v) / 1000000).toFixed(1) + 'M' } } } }
    });

    new Chart(mc, {
      type: 'doughnut',
      data: { labels: d.revenueByModule.map(m => m.module), datasets: [{ data: d.revenueByModule.map(m => m.amount), backgroundColor: ['#f59e0b', '#0ea5e9', '#10b981'], borderWidth: 0 }] },
      options: { responsive: true, cutout: '65%', plugins: { legend: { position: 'bottom' } } }
    });
  }
}
TSEOF

# ============================================================
# MARKETING
# ============================================================
mkdir -p "$BASE/marketing"

cat > "$BASE/marketing/marketing.component.ts" << 'TSEOF'
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PromoCodeDto } from '../../core/models/api.models';

@Component({
  selector: 'app-marketing',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Marketing & Promotions</h4>
        <p class="text-muted mb-0">Gérez vos codes promo et campagnes</p>
      </div>
      <button class="btn btn-primary" (click)="showCreateForm.set(true)">
        <i class="bi bi-plus-lg me-2"></i>Nouveau code promo
      </button>
    </div>

    <!-- Create form -->
    @if (showCreateForm()) {
      <div class="table-card mb-4">
        <div class="card-header"><h6 class="fw-bold mb-0">Nouveau code promo</h6></div>
        <div class="card-body p-3">
          <div class="row g-3">
            <div class="col-md-3">
              <label class="form-label small fw-medium">Code</label>
              <input type="text" class="form-control" [(ngModel)]="newPromo.code" placeholder="ex: WELCOME20" />
            </div>
            <div class="col-md-2">
              <label class="form-label small fw-medium">Type</label>
              <select class="form-select" [(ngModel)]="newPromo.type">
                <option value="Percentage">Pourcentage</option>
                <option value="Fixed">Montant fixe</option>
                <option value="FreeDelivery">Livraison gratuite</option>
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label small fw-medium">Valeur</label>
              <input type="number" class="form-control" [(ngModel)]="newPromo.value" />
            </div>
            <div class="col-md-2">
              <label class="form-label small fw-medium">Commande min (MAD)</label>
              <input type="number" class="form-control" [(ngModel)]="newPromo.minOrder" />
            </div>
            <div class="col-md-3">
              <label class="form-label small fw-medium">Limite d'utilisation</label>
              <input type="number" class="form-control" [(ngModel)]="newPromo.usageLimit" />
            </div>
            <div class="col-md-3">
              <label class="form-label small fw-medium">Date début</label>
              <input type="date" class="form-control" [(ngModel)]="newPromo.startDate" />
            </div>
            <div class="col-md-3">
              <label class="form-label small fw-medium">Date fin</label>
              <input type="date" class="form-control" [(ngModel)]="newPromo.endDate" />
            </div>
            <div class="col-md-6 d-flex align-items-end gap-2">
              <button class="btn btn-primary" (click)="createPromo()"><i class="bi bi-check-lg me-1"></i>Créer</button>
              <button class="btn btn-light" (click)="showCreateForm.set(false)">Annuler</button>
            </div>
          </div>
        </div>
      </div>
    }

    <!-- Promo Codes Table -->
    <div class="table-card">
      <div class="card-header">
        <h6 class="fw-bold mb-0">Codes promo actifs</h6>
      </div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead>
            <tr><th>Code</th><th>Type</th><th>Valeur</th><th>Min commande</th><th>Utilisations</th><th>Période</th><th>Statut</th><th class="text-end">Actions</th></tr>
          </thead>
          <tbody>
            @for (p of promos(); track p.id) {
              <tr>
                <td><span class="fw-bold font-monospace text-primary">{{ p.code }}</span></td>
                <td><span class="badge bg-light text-dark">{{ p.type }}</span></td>
                <td class="fw-medium">
                  @if (p.type === 'Percentage') { {{ p.value }}% }
                  @else if (p.type === 'Fixed') { {{ p.value }} MAD }
                  @else { Gratuite }
                </td>
                <td>{{ p.minOrder | number }} MAD</td>
                <td>{{ p.usedCount }} / {{ p.usageLimit }}</td>
                <td class="small text-muted">{{ p.startDate | date:'dd/MM' }} → {{ p.endDate | date:'dd/MM' }}</td>
                <td>
                  @if (p.isActive) {
                    <span class="badge-status bg-success-subtle text-success">Actif</span>
                  } @else {
                    <span class="badge-status bg-secondary-subtle text-secondary">Expiré</span>
                  }
                </td>
                <td class="text-end">
                  <button class="btn btn-sm btn-outline-danger" (click)="togglePromo(p)">
                    @if (p.isActive) { <i class="bi bi-pause"></i> } @else { <i class="bi bi-play"></i> }
                  </button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class MarketingComponent implements OnInit {
  promos = signal<PromoCodeDto[]>([]);
  showCreateForm = signal(false);
  newPromo = { code: '', type: 'Percentage', value: 0, minOrder: 0, usageLimit: 100, startDate: '', endDate: '' };

  constructor(private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadPromos(); }

  loadPromos(): void {
    this.api.get<PromoCodeDto[]>('admin/promo-codes').subscribe({
      next: res => { if (res.success) this.promos.set(res.data); },
      error: () => {
        this.promos.set([
          { id: '1', code: 'WELCOME20', type: 'Percentage', value: 20, minOrder: 50, maxDiscount: 100, usageLimit: 1000, usedCount: 342, startDate: '2025-01-01', endDate: '2025-12-31', isActive: true },
          { id: '2', code: 'LIVGRATUITE', type: 'FreeDelivery', value: 0, minOrder: 100, maxDiscount: 0, usageLimit: 500, usedCount: 189, startDate: '2025-06-01', endDate: '2025-06-30', isActive: true },
          { id: '3', code: 'ETE50', type: 'Fixed', value: 50, minOrder: 200, maxDiscount: 50, usageLimit: 200, usedCount: 200, startDate: '2025-05-01', endDate: '2025-05-31', isActive: false },
        ]);
      }
    });
  }

  createPromo(): void {
    this.api.post('admin/promo-codes', this.newPromo).subscribe({
      next: () => { this.toast.success('Code promo créé'); this.showCreateForm.set(false); this.loadPromos(); },
      error: () => this.toast.error('Erreur lors de la création')
    });
  }

  togglePromo(p: PromoCodeDto): void {
    this.api.patch(`admin/promo-codes/${p.id}/toggle`, {}).subscribe({
      next: () => { this.toast.success('Statut mis à jour'); this.loadPromos(); }
    });
  }
}
TSEOF

# ============================================================
# SETTINGS
# ============================================================
mkdir -p "$BASE/settings"

cat > "$BASE/settings/settings.component.ts" << 'TSEOF'
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../core/services/toast.service';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="mb-4">
      <h4 class="fw-bold mb-1">Configuration</h4>
      <p class="text-muted mb-0">Paramètres généraux de la plateforme</p>
    </div>

    <!-- Tabs -->
    <ul class="nav nav-tabs mb-4">
      @for (tab of settingsTabs; track tab.id) {
        <li class="nav-item">
          <a class="nav-link" [class.active]="activeTab() === tab.id" (click)="activeTab.set(tab.id)" style="cursor:pointer">
            <i class="bi me-1" [class]="tab.icon"></i>{{ tab.label }}
          </a>
        </li>
      }
    </ul>

    <!-- Commissions -->
    @if (activeTab() === 'commissions') {
      <div class="table-card">
        <div class="card-header"><h6 class="fw-bold mb-0">Taux de commission par module</h6></div>
        <div class="card-body p-4">
          <div class="row g-4">
            <div class="col-md-4">
              <div class="p-3 rounded-3 border">
                <div class="d-flex align-items-center gap-2 mb-3">
                  <i class="bi bi-shop text-warning fs-4"></i>
                  <h6 class="fw-bold mb-0">Restaurants</h6>
                </div>
                <label class="form-label small">Commission (%)</label>
                <input type="number" class="form-control" [(ngModel)]="commissions.restaurant" min="0" max="50" />
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-3 rounded-3 border">
                <div class="d-flex align-items-center gap-2 mb-3">
                  <i class="bi bi-tools text-info fs-4"></i>
                  <h6 class="fw-bold mb-0">Services</h6>
                </div>
                <label class="form-label small">Commission (%)</label>
                <input type="number" class="form-control" [(ngModel)]="commissions.service" min="0" max="50" />
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-3 rounded-3 border">
                <div class="d-flex align-items-center gap-2 mb-3">
                  <i class="bi bi-cart3 text-success fs-4"></i>
                  <h6 class="fw-bold mb-0">Courses</h6>
                </div>
                <label class="form-label small">Commission (%)</label>
                <input type="number" class="form-control" [(ngModel)]="commissions.grocery" min="0" max="50" />
              </div>
            </div>
          </div>
          <div class="mt-4">
            <button class="btn btn-primary" (click)="saveCommissions()"><i class="bi bi-check-lg me-2"></i>Sauvegarder</button>
          </div>
        </div>
      </div>
    }

    <!-- Zones -->
    @if (activeTab() === 'zones') {
      <div class="table-card">
        <div class="card-header">
          <h6 class="fw-bold mb-0">Zones géographiques</h6>
          <button class="btn btn-sm btn-primary"><i class="bi bi-plus-lg me-1"></i>Ajouter</button>
        </div>
        <div class="card-body p-4">
          <div class="row g-3">
            @for (zone of zones; track zone.name) {
              <div class="col-md-4">
                <div class="p-3 rounded-3 border">
                  <h6 class="fw-bold">{{ zone.name }}</h6>
                  <p class="text-muted small mb-2">Frais de livraison: {{ zone.deliveryFee }} MAD</p>
                  <p class="text-muted small mb-0">Supplément: {{ zone.surcharge }} MAD</p>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    }

    <!-- Payment -->
    @if (activeTab() === 'payment') {
      <div class="table-card">
        <div class="card-header"><h6 class="fw-bold mb-0">Configuration paiement</h6></div>
        <div class="card-body p-4">
          <div class="row g-4">
            <div class="col-md-6">
              <label class="form-label fw-medium">Clé API Stripe</label>
              <input type="password" class="form-control" value="sk_live_•••••••••••" disabled />
            </div>
            <div class="col-md-6">
              <label class="form-label fw-medium">Webhook Secret</label>
              <input type="password" class="form-control" value="whsec_•••••••••••" disabled />
            </div>
          </div>
          <div class="mt-3">
            <div class="form-check form-switch">
              <input class="form-check-input" type="checkbox" checked id="enableCash">
              <label class="form-check-label" for="enableCash">Autoriser paiement en espèces</label>
            </div>
            <div class="form-check form-switch mt-2">
              <input class="form-check-input" type="checkbox" checked id="enableWallet">
              <label class="form-check-label" for="enableWallet">Activer le portefeuille virtuel</label>
            </div>
            <div class="form-check form-switch mt-2">
              <input class="form-check-input" type="checkbox" checked id="enable3ds">
              <label class="form-check-label" for="enable3ds">3D Secure obligatoire</label>
            </div>
          </div>
        </div>
      </div>
    }

    <!-- System -->
    @if (activeTab() === 'system') {
      <div class="table-card">
        <div class="card-header"><h6 class="fw-bold mb-0">Système</h6></div>
        <div class="card-body p-4">
          <div class="row g-3 mb-4">
            <div class="col-md-4"><strong>Version API:</strong> 1.0.0</div>
            <div class="col-md-4"><strong>Environnement:</strong> Production</div>
            <div class="col-md-4"><strong>Base de données:</strong> PostgreSQL 16</div>
          </div>
          <div class="row g-3">
            <div class="col-md-4"><strong>Cache Redis:</strong> <span class="text-success">Connecté</span></div>
            <div class="col-md-4"><strong>Stripe:</strong> <span class="text-success">Connecté</span></div>
            <div class="col-md-4"><strong>Firebase:</strong> <span class="text-success">Connecté</span></div>
          </div>
          <hr>
          <button class="btn btn-outline-warning me-2"><i class="bi bi-arrow-repeat me-1"></i>Vider le cache</button>
          <button class="btn btn-outline-danger"><i class="bi bi-database me-1"></i>Logs système</button>
        </div>
      </div>
    }
  `
})
export class SettingsComponent {
  activeTab = signal('commissions');
  settingsTabs = [
    { id: 'commissions', label: 'Commissions', icon: 'bi-percent' },
    { id: 'zones', label: 'Zones', icon: 'bi-geo-alt' },
    { id: 'payment', label: 'Paiement', icon: 'bi-credit-card' },
    { id: 'system', label: 'Système', icon: 'bi-gear' },
  ];

  commissions = { restaurant: 15, service: 12, grocery: 8 };
  zones = [
    { name: 'Casablanca Centre', deliveryFee: 15, surcharge: 0 },
    { name: 'Casablanca Périphérie', deliveryFee: 25, surcharge: 5 },
    { name: 'Rabat Centre', deliveryFee: 15, surcharge: 0 },
    { name: 'Rabat Périphérie', deliveryFee: 25, surcharge: 5 },
    { name: 'Marrakech', deliveryFee: 20, surcharge: 0 },
  ];

  constructor(private api: ApiService, private toast: ToastService) {}

  saveCommissions(): void {
    this.api.put('admin/settings/commissions', this.commissions).subscribe({
      next: () => this.toast.success('Commissions sauvegardées'),
      error: () => this.toast.error('Erreur')
    });
  }
}
TSEOF

echo "✅ Orders + Delivery + Finance + Marketing + Settings generated!"
