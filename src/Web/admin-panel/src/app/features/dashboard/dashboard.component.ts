import { Component, OnInit, signal, ElementRef, viewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { DashboardStats, RecentOrder } from '../../core/models/api.models';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DecimalPipe, DatePipe, RouterLink, StatCardComponent, LoadingSpinnerComponent, StatusBadgePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  revenueChartRef = viewChild<ElementRef>('revenueChart');
  ordersChartRef = viewChild<ElementRef>('ordersChart');

  loading = signal(true);
  selectedPeriod = signal('week');
  recentOrders = signal<RecentOrder[]>([]);

  stats = signal<DashboardStats>({
    totalOrders: 0, totalOrdersToday: 0, totalRevenue: 0, revenueToday: 0,
    activeClients: 0, activeRestaurants: 0, activeServiceProviders: 0,
    activeGroceryStores: 0, activeDeliverers: 0, pendingApprovals: 0, openTickets: 0,
    revenueChart: [], orderChart: [],
    restaurantStats: { totalOrders: 0, revenue: 0, commission: 0, avgRating: 0 },
    serviceStats: { totalOrders: 0, revenue: 0, commission: 0, avgRating: 0 },
    groceryStats: { totalOrders: 0, revenue: 0, commission: 0, avgRating: 0 }
  });

  private revenueChart: Chart | null = null;
  private ordersChart: Chart | null = null;

  periods = [
    { value: 'today', label: "Aujourd'hui" },
    { value: 'week', label: 'Cette semaine' },
    { value: 'month', label: 'Ce mois' },
    { value: 'year', label: 'Cette année' }
  ];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadData();
    this.loadRecentOrders();
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.initCharts(), 600);
  }

  ngOnDestroy(): void {
    this.revenueChart?.destroy();
    this.ordersChart?.destroy();
  }

  onPeriodChange(period: string): void {
    this.selectedPeriod.set(period);
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    const period = this.selectedPeriod();
    const now = new Date();
    let from: Date;
    switch (period) {
      case 'today': from = new Date(now.getFullYear(), now.getMonth(), now.getDate()); break;
      case 'week': from = new Date(now.getTime() - 7 * 86400000); break;
      case 'month': from = new Date(now.getFullYear(), now.getMonth(), 1); break;
      case 'year': from = new Date(now.getFullYear(), 0, 1); break;
      default: from = new Date(now.getTime() - 7 * 86400000);
    }

    this.api.get<DashboardStats>('admin/dashboard', {
      from: from.toISOString(), to: now.toISOString()
    }).subscribe({
      next: res => {
        if (res.success) {
          this.stats.set(res.data);
          setTimeout(() => this.initCharts(), 100);
        }
        this.loading.set(false);
      },
      error: () => {
        this.setDemoData();
        this.loading.set(false);
      }
    });
  }

  loadRecentOrders(): void {
    this.api.get<{ items: RecentOrder[] }>('admin/orders', {
      page: 1, pageSize: 5, sortBy: 'createdAt', sortOrder: 'desc'
    }).subscribe({
      next: res => {
        if (res.success && res.data?.items) {
          this.recentOrders.set(res.data.items);
        }
      },
      error: () => {
        this.recentOrders.set([
          { id: '1', orderNumber: 'CMD-8854', type: 'Restaurant', customerName: 'Ahmed B.', providerName: 'Chez Ali', amount: 185.50, status: 'Delivered', createdAt: new Date().toISOString() },
          { id: '2', orderNumber: 'SRV-2241', type: 'Service', customerName: 'Fatima Z.', providerName: 'ProClean', amount: 450.00, status: 'InProgress', createdAt: new Date().toISOString() },
          { id: '3', orderNumber: 'GRC-1102', type: 'Grocery', customerName: 'Youssef M.', providerName: 'Carrefour', amount: 324.75, status: 'Preparing', createdAt: new Date().toISOString() },
          { id: '4', orderNumber: 'CMD-8853', type: 'Restaurant', customerName: 'Sara L.', providerName: 'Pizza Roma', amount: 98.00, status: 'InTransit', createdAt: new Date().toISOString() },
          { id: '5', orderNumber: 'SRV-2240', type: 'Service', customerName: 'Omar K.', providerName: 'FixIt Pro', amount: 800.00, status: 'Confirmed', createdAt: new Date().toISOString() },
        ]);
      }
    });
  }

  getTypeIcon(type: string): string {
    return { Restaurant: 'bi-shop', Service: 'bi-tools', Grocery: 'bi-cart3' }[type] ?? 'bi-box';
  }

  getTypeClass(type: string): string {
    return { Restaurant: 'type-restaurant', Service: 'type-service', Grocery: 'type-grocery' }[type] ?? '';
  }

  private initCharts(): void {
    const rc = this.revenueChartRef()?.nativeElement;
    const oc = this.ordersChartRef()?.nativeElement;
    if (!rc || !oc) return;

    const data = this.stats();
    this.revenueChart?.destroy();
    this.ordersChart?.destroy();

    // ── Revenue Chart (multi-series stacked bar) ──
    const labels = data.revenueChart.map(d => {
      if (!d.date) return '?';
      const dt = new Date(d.date);
      return dt.toLocaleDateString('fr-FR', { weekday: 'short' });
    });

    this.revenueChart = new Chart(rc, {
      type: 'bar',
      data: {
        labels,
        datasets: [
          {
            label: 'Restaurants',
            data: data.revenueChart.map(d => d.restaurant),
            backgroundColor: 'rgba(245, 158, 11, 0.7)',
            borderRadius: 4,
            barPercentage: 0.7
          },
          {
            label: 'Services',
            data: data.revenueChart.map(d => d.service),
            backgroundColor: 'rgba(99, 102, 241, 0.7)',
            borderRadius: 4,
            barPercentage: 0.7
          },
          {
            label: 'Courses',
            data: data.revenueChart.map(d => d.grocery),
            backgroundColor: 'rgba(16, 185, 129, 0.7)',
            borderRadius: 4,
            barPercentage: 0.7
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'top', labels: { usePointStyle: true, pointStyle: 'circle', padding: 16, font: { size: 12 } } }
        },
        scales: {
          x: { stacked: true, grid: { display: false } },
          y: {
            stacked: true,
            beginAtZero: true,
            grid: { color: 'rgba(0,0,0,0.04)' },
            ticks: {
              callback: (v: any) => {
                const num = Number(v);
                if (num >= 1000) return (num / 1000).toFixed(0) + 'K';
                return num.toString();
              }
            }
          }
        }
      }
    });

    // ── Orders Doughnut (from module stats) ──
    const restOrders = data.restaurantStats?.totalOrders ?? 0;
    const svcOrders = data.serviceStats?.totalOrders ?? 0;
    const grcOrders = data.groceryStats?.totalOrders ?? 0;
    const hasOrderData = restOrders + svcOrders + grcOrders > 0;

    this.ordersChart = new Chart(oc, {
      type: 'doughnut',
      data: {
        labels: ['Restaurants', 'Services', 'Courses'],
        datasets: [{
          data: hasOrderData ? [restOrders, svcOrders, grcOrders] : [1, 1, 1],
          backgroundColor: hasOrderData
            ? ['#f59e0b', '#6366f1', '#10b981']
            : ['#e2e8f0', '#e2e8f0', '#e2e8f0'],
          borderWidth: 0,
          spacing: 3
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%',
        plugins: {
          legend: {
            position: 'bottom',
            labels: { padding: 16, usePointStyle: true, pointStyle: 'circle', font: { size: 12 } }
          }
        }
      }
    });
  }

  private setDemoData(): void {
    this.stats.set({
      totalOrders: 15420, totalOrdersToday: 342, totalRevenue: 2847500, revenueToday: 185000,
      activeClients: 45200, activeRestaurants: 850, activeServiceProviders: 320,
      activeGroceryStores: 80, activeDeliverers: 340, pendingApprovals: 12, openTickets: 5,
      revenueChart: [
        { date: '2026-02-09', restaurant: 180000, service: 95000, grocery: 105000 },
        { date: '2026-02-10', restaurant: 210000, service: 88000, grocery: 122000 },
        { date: '2026-02-11', restaurant: 165000, service: 102000, grocery: 83000 },
        { date: '2026-02-12', restaurant: 245000, service: 115000, grocery: 150000 },
        { date: '2026-02-13', restaurant: 290000, service: 140000, grocery: 190000 },
        { date: '2026-02-14', restaurant: 225000, service: 98000, grocery: 127000 },
        { date: '2026-02-15', restaurant: 155000, service: 72000, grocery: 93000 },
      ],
      orderChart: [
        { date: '2026-02-09', restaurant: 120, service: 45, grocery: 55 },
        { date: '2026-02-10', restaurant: 145, service: 52, grocery: 68 },
        { date: '2026-02-11', restaurant: 110, service: 48, grocery: 42 },
        { date: '2026-02-12', restaurant: 168, service: 61, grocery: 85 },
        { date: '2026-02-13', restaurant: 195, service: 72, grocery: 98 },
        { date: '2026-02-14', restaurant: 150, service: 55, grocery: 70 },
        { date: '2026-02-15', restaurant: 98, service: 38, grocery: 52 },
      ],
      restaurantStats: { totalOrders: 8500, revenue: 1470000, commission: 220500, avgRating: 4.3 },
      serviceStats: { totalOrders: 3200, revenue: 710000, commission: 106500, avgRating: 4.5 },
      groceryStats: { totalOrders: 3720, revenue: 667500, commission: 66750, avgRating: 4.1 }
    });
    setTimeout(() => this.initCharts(), 100);
  }
}