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
