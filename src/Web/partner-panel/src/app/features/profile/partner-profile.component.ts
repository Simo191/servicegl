import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-profile',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h4 class="fw-bold mb-4">Mon Profil</h4>
    <div class="table-card">
      <div class="card-body p-4">
        <div class="row g-3">
          <div class="col-md-6"><label class="form-label fw-medium">Nom de l'entreprise</label><input class="form-control" [value]="auth.currentUser()?.businessName" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Responsable</label><input class="form-control" [value]="auth.currentUser()?.name" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Email</label><input class="form-control" [value]="auth.currentUser()?.email" /></div>
          <div class="col-md-6"><label class="form-label fw-medium">Téléphone</label><input class="form-control" value="+212600000000" /></div>
          <div class="col-12"><label class="form-label fw-medium">Adresse</label><input class="form-control" value="12 Rue Mohammed V, Casablanca" /></div>
          <div class="col-12"><label class="form-label fw-medium">Description</label><textarea class="form-control" rows="3">Restaurant traditionnel marocain depuis 2005.</textarea></div>
          <div class="col-12"><button class="btn btn-primary" (click)="toast.success('Profil mis à jour')"><i class="bi bi-check-lg me-2"></i>Sauvegarder</button></div>
        </div>
      </div>
    </div>
  `
})
export class PartnerProfileComponent {
  constructor(public auth: AuthService, public toast: ToastService) {}
}
