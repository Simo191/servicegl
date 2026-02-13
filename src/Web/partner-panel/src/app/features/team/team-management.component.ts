import { Component, signal } from '@angular/core';
@Component({
  selector: 'app-team-management',
  standalone: true,
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mon Équipe</h4>
      <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Ajouter un intervenant</button>
    </div>
    <div class="row g-3">
      @for (m of members(); track m.id) {
        <div class="col-md-4">
          <div class="table-card text-center p-4">
            <div class="rounded-circle bg-primary bg-opacity-10 text-primary d-inline-flex align-items-center justify-content-center mb-2" style="width:60px;height:60px;font-size:1.2rem;font-weight:600">{{ m.initials }}</div>
            <h6 class="fw-bold mb-0">{{ m.name }}</h6>
            <small class="text-muted">{{ m.role }}</small>
            <div class="mt-2">
              @if (m.available) { <span class="badge bg-success">Disponible</span> }
              @else { <span class="badge bg-secondary">Occupé</span> }
            </div>
            <div class="mt-2 small text-muted">{{ m.interventions }} interventions • {{ m.rating }} ★</div>
          </div>
        </div>
      }
    </div>
  `
})
export class TeamManagementComponent {
  members = signal([
    { id: '1', name: 'Rachid M.', initials: 'RM', role: 'Plombier senior', available: true, interventions: 180, rating: 4.8 },
    { id: '2', name: 'Samir B.', initials: 'SB', role: 'Plombier', available: false, interventions: 95, rating: 4.5 },
    { id: '3', name: 'Hassan K.', initials: 'HK', role: 'Apprenti', available: true, interventions: 30, rating: 4.2 },
  ]);
}
