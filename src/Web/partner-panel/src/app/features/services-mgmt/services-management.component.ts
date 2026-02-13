import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-services-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mes Services</h4>
      <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Ajouter un service</button>
    </div>
    <div class="row g-3">
      @for (s of services(); track s.id) {
        <div class="col-md-6">
          <div class="table-card">
            <div class="card-body p-3">
              <div class="d-flex justify-content-between">
                <h6 class="fw-bold mb-1">{{ s.name }}</h6>
                <div class="form-check form-switch">
                  <input class="form-check-input" type="checkbox" [checked]="s.active">
                </div>
              </div>
              <p class="text-muted small mb-2">{{ s.description }}</p>
              <div class="d-flex gap-3 small">
                <span><i class="bi bi-clock me-1"></i>{{ s.duration }}</span>
                <span class="fw-bold text-primary">
                  @if (s.priceType === 'hourly') { {{ s.price | number }} MAD/h }
                  @else if (s.priceType === 'fixed') { {{ s.price | number }} MAD }
                  @else { Sur devis }
                </span>
              </div>
              <div class="mt-2">
                <button class="btn btn-sm btn-outline-primary me-1"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class ServicesManagementComponent {
  services = signal([
    { id: '1', name: 'Réparation fuite', description: 'Détection et réparation de fuites d\'eau', duration: '1-3h', price: 200, priceType: 'hourly', active: true },
    { id: '2', name: 'Débouchage canalisation', description: 'Débouchage professionnel', duration: '1-2h', price: 350, priceType: 'fixed', active: true },
    { id: '3', name: 'Installation sanitaire', description: 'Installation complète salle de bain', duration: '4-8h', price: 0, priceType: 'quote', active: true },
    { id: '4', name: 'Réparation chauffe-eau', description: 'Diagnostic et réparation', duration: '1-3h', price: 250, priceType: 'fixed', active: false },
  ]);
}
