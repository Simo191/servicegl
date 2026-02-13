import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center" style="background:linear-gradient(135deg,#0ea5e9,#4f46e5)">
      <div class="card border-0 shadow-lg" style="width:420px;border-radius:16px">
        <div class="card-body p-5">
          <div class="text-center mb-4">
            <div class="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 rounded-circle mb-3" style="width:64px;height:64px">
              <i class="bi bi-building text-primary fs-3"></i>
            </div>
            <h4 class="fw-bold">Espace Partenaire</h4>
            <p class="text-muted">Restaurant • Service • Magasin</p>
          </div>
          <div class="mb-3">
            <label class="form-label fw-medium">Email</label>
            <input type="email" class="form-control" [(ngModel)]="email" placeholder="partenaire@email.com" />
          </div>
          <div class="mb-4">
            <label class="form-label fw-medium">Mot de passe</label>
            <input type="password" class="form-control" [(ngModel)]="password" placeholder="••••••••" />
          </div>
          <button class="btn btn-primary w-100 py-2 fw-medium" [disabled]="loading()" (click)="login()">
            @if (loading()) { <span class="spinner-border spinner-border-sm me-2"></span> }
            Se connecter
          </button>
        </div>
      </div>
    </div>
  `
})
export class PartnerLoginComponent {
  email = ''; password = ''; loading = signal(false);
  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}
  login(): void {
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: r => { if (r.success) { this.router.navigate(['/dashboard']); } else { this.toast.error(r.message); } this.loading.set(false); },
      error: () => { this.toast.error('Identifiants invalides'); this.loading.set(false); }
    });
  }
}
