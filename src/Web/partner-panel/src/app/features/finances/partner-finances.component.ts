import { Component, signal } from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common';
@Component({
  selector: 'app-partner-finances',
  standalone: true,
  imports: [DecimalPipe, DatePipe],
  template: `
    <h4 class="fw-bold mb-4">Mes Finances</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Solde disponible</p><h3 class="stat-value text-success">{{ balance() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">CA ce mois</p><h3 class="stat-value">{{ monthRevenue() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Commissions</p><h3 class="stat-value text-danger">-{{ commissions() | number:'1.0-0' }} MAD</h3></div></div>
      <div class="col-md-3"><div class="stat-card"><p class="stat-label mb-1">Total viré</p><h3 class="stat-value">{{ totalPaid() | number:'1.0-0' }} MAD</h3></div></div>
    </div>
    <div class="d-flex gap-2 mb-4">
      <button class="btn btn-primary"><i class="bi bi-send me-2"></i>Demander un virement</button>
      <button class="btn btn-outline-primary"><i class="bi bi-download me-2"></i>Export Excel</button>
    </div>
    <div class="table-card">
      <div class="card-header"><h6 class="fw-bold mb-0">Historique des paiements</h6></div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Date</th><th>Référence</th><th>Type</th><th>Montant</th><th>Statut</th></tr></thead>
          <tbody>
            @for (p of payments(); track p.ref) {
              <tr>
                <td class="text-muted">{{ p.date }}</td>
                <td class="fw-medium">{{ p.ref }}</td>
                <td>{{ p.type }}</td>
                <td class="fw-bold" [class.text-success]="p.amount > 0" [class.text-danger]="p.amount < 0">{{ p.amount > 0 ? '+' : '' }}{{ p.amount | number:'1.2-2' }} MAD</td>
                <td><span class="badge" [class]="p.statusClass">{{ p.status }}</span></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class PartnerFinancesComponent {
  balance = signal(15250); monthRevenue = signal(42000); commissions = signal(6300); totalPaid = signal(128000);
  payments = signal([
    { date: '02/06/2025', ref: 'VIR-1045', type: 'Virement', amount: 8500, status: 'Effectué', statusClass: 'bg-success' },
    { date: '25/05/2025', ref: 'VIR-1038', type: 'Virement', amount: 12000, status: 'Effectué', statusClass: 'bg-success' },
    { date: '02/06/2025', ref: 'COM-8854', type: 'Commission', amount: -27.83, status: 'Prélevée', statusClass: 'bg-secondary' },
    { date: '01/06/2025', ref: 'COM-8850', type: 'Commission', amount: -35.25, status: 'Prélevée', statusClass: 'bg-secondary' },
  ]);
}
