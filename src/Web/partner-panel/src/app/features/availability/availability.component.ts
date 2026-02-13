import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h4 class="fw-bold mb-4">Gestion des Disponibilités</h4>
    <div class="table-card mb-4">
      <div class="card-header"><h6 class="fw-bold mb-0">Horaires de travail</h6></div>
      <div class="card-body p-3">
        @for (day of days; track day.name) {
          <div class="d-flex align-items-center gap-3 py-2 border-bottom">
            <div style="width:100px" class="fw-medium">{{ day.name }}</div>
            <div class="form-check form-switch">
              <input class="form-check-input" type="checkbox" [(ngModel)]="day.active">
            </div>
            @if (day.active) {
              <input type="time" class="form-control form-control-sm" style="width:120px" [(ngModel)]="day.start" />
              <span>→</span>
              <input type="time" class="form-control form-control-sm" style="width:120px" [(ngModel)]="day.end" />
            } @else {
              <span class="text-muted small">Indisponible</span>
            }
          </div>
        }
        <button class="btn btn-primary btn-sm mt-3" (click)="save()"><i class="bi bi-check-lg me-1"></i>Sauvegarder</button>
      </div>
    </div>

    <div class="table-card">
      <div class="card-header">
        <h6 class="fw-bold mb-0">Mode indisponible</h6>
        <div class="form-check form-switch">
          <input class="form-check-input" type="checkbox" [(ngModel)]="unavailable">
          <label class="form-check-label">{{ unavailable ? 'Indisponible' : 'Disponible' }}</label>
        </div>
      </div>
    </div>
  `
})
export class AvailabilityComponent {
  unavailable = false;
  days = [
    { name: 'Lundi', active: true, start: '08:00', end: '18:00' },
    { name: 'Mardi', active: true, start: '08:00', end: '18:00' },
    { name: 'Mercredi', active: true, start: '08:00', end: '18:00' },
    { name: 'Jeudi', active: true, start: '08:00', end: '18:00' },
    { name: 'Vendredi', active: true, start: '08:00', end: '18:00' },
    { name: 'Samedi', active: true, start: '09:00', end: '14:00' },
    { name: 'Dimanche', active: false, start: '', end: '' },
  ];
  constructor(private toast: ToastService) {}
  save(): void { this.toast.success('Horaires sauvegardés'); }
}
