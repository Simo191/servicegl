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
