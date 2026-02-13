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
