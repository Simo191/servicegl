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
