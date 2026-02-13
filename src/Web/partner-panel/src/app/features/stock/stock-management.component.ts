import { Component, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
@Component({
  selector: 'app-stock-management',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <h4 class="fw-bold mb-4">Gestion du Stock</h4>
    <div class="row g-3 mb-4">
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-primary">12,500</h3><small class="text-muted">Produits total</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-success">11,850</h3><small class="text-muted">En stock</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-warning">42</h3><small class="text-muted">Stock faible</small></div></div>
      <div class="col-md-3"><div class="stat-card text-center"><h3 class="fw-bold text-danger">15</h3><small class="text-muted">Rupture</small></div></div>
    </div>
    <div class="table-card">
      <div class="card-header"><h6 class="fw-bold mb-0">⚠️ Alertes stock</h6></div>
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Produit</th><th>Stock actuel</th><th>Seuil alerte</th><th>Statut</th><th class="text-end">Action</th></tr></thead>
          <tbody>
            @for (a of alerts(); track a.name) {
              <tr>
                <td class="fw-medium">{{ a.name }}</td>
                <td [class.text-danger]="a.stock === 0" class="fw-bold">{{ a.stock }}</td>
                <td>{{ a.threshold }}</td>
                <td>@if (a.stock === 0) { <span class="badge bg-danger">Rupture</span> } @else { <span class="badge bg-warning">Faible</span> }</td>
                <td class="text-end"><button class="btn btn-sm btn-outline-primary">Réapprovisionner</button></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class StockManagementComponent {
  alerts = signal([
    { name: 'Eau Sidi Ali 1.5L', stock: 0, threshold: 50 },
    { name: 'Huile d\'olive 1L', stock: 8, threshold: 20 },
    { name: 'Beurre Président', stock: 5, threshold: 15 },
    { name: 'Oeufs (douzaine)', stock: 12, threshold: 30 },
  ]);
}
