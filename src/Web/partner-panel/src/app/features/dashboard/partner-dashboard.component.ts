import { Component, OnInit, signal, viewChild, ElementRef, AfterViewInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-partner-dashboard',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <div class="mb-4">
      <h4 class="fw-bold mb-1">Bonjour, {{ auth.currentUser()?.name }} 👋</h4>
      <p class="text-muted mb-0">Voici le résumé de votre activité</p>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">CA du jour</p>
          <h3 class="stat-value">{{ todayRevenue() | number:'1.0-0' }} <small class="fs-6 fw-normal">MAD</small></h3>
          <span class="stat-change text-success"><i class="bi bi-arrow-up"></i> +12%</span>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Commandes du jour</p>
          <h3 class="stat-value">{{ todayOrders() }}</h3>
          <span class="stat-change text-success"><i class="bi bi-arrow-up"></i> +8%</span>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Note moyenne</p>
          <h3 class="stat-value">4.6 <small class="fs-6 text-warning">★</small></h3>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card">
          <p class="stat-label mb-1">Commandes en cours</p>
          <h3 class="stat-value text-primary">{{ activeOrders() }}</h3>
        </div>
      </div>
    </div>

    <div class="row g-3 mb-4">
      <div class="col-lg-8">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Revenus - 7 derniers jours</h6></div>
          <div class="p-3"><canvas #weekChart height="280"></canvas></div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="table-card">
          <div class="card-header"><h6 class="fw-bold mb-0">Commandes récentes</h6></div>
          <div class="list-group list-group-flush">
            @for (o of recentOrders(); track o.id) {
              <div class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div class="fw-medium small">#{{ o.number }}</div>
                  <small class="text-muted">{{ o.customer }}</small>
                </div>
                <div class="text-end">
                  <div class="fw-bold small">{{ o.amount }} MAD</div>
                  <span class="badge" [class]="o.statusClass">{{ o.statusLabel }}</span>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  `
})
export class PartnerDashboardComponent implements OnInit, AfterViewInit {
  weekChartRef = viewChild<ElementRef>('weekChart');
  todayRevenue = signal(4250); todayOrders = signal(28); activeOrders = signal(5);
  recentOrders = signal([
    { id: '1', number: 'CMD-452', customer: 'Ahmed B.', amount: 185, statusLabel: 'En cours', statusClass: 'bg-primary' },
    { id: '2', number: 'CMD-451', customer: 'Sara L.', amount: 98, statusLabel: 'Prête', statusClass: 'bg-success' },
    { id: '3', number: 'CMD-450', customer: 'Omar K.', amount: 245, statusLabel: 'Livrée', statusClass: 'bg-secondary' },
    { id: '4', number: 'CMD-449', customer: 'Fatima Z.', amount: 132, statusLabel: 'Livrée', statusClass: 'bg-secondary' },
  ]);

  constructor(public auth: AuthService, private api: ApiService) {}
  ngOnInit(): void {}
  ngAfterViewInit(): void { setTimeout(() => this.initChart(), 200); }

  private initChart(): void {
    const el = this.weekChartRef()?.nativeElement;
    if (!el) return;
    new Chart(el, {
      type: 'bar',
      data: {
        labels: ['Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam', 'Dim'],
        datasets: [{ label: 'CA (MAD)', data: [3200, 4100, 3800, 5200, 6100, 4800, 3500], backgroundColor: 'rgba(79,70,229,0.2)', borderColor: '#4f46e5', borderWidth: 2, borderRadius: 8 }]
      },
      options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
    });
  }
}
