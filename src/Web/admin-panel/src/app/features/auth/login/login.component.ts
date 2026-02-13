import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center" style="background:linear-gradient(135deg,#667eea 0%,#764ba2 100%)">
      <div class="card border-0 shadow-lg" style="width:420px;border-radius:16px">
        <div class="card-body p-5">
          <div class="text-center mb-4">
            <div class="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 rounded-circle mb-3" style="width:64px;height:64px">
              <i class="bi bi-grid-1x2-fill text-primary fs-3"></i>
            </div>
            <h4 class="fw-bold">MultiServices Admin</h4>
            <p class="text-muted">Connectez-vous à votre compte</p>
          </div>

          <div class="mb-3">
            <label class="form-label fw-medium">Email</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-envelope"></i></span>
              <input type="email" class="form-control" [(ngModel)]="email" placeholder="admin@multiservices.ma" />
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-medium">Mot de passe</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-lock"></i></span>
              <input [type]="showPass() ? 'text' : 'password'" class="form-control" [(ngModel)]="password" placeholder="••••••••" />
              <button class="btn btn-outline-secondary" type="button" (click)="showPass.set(!showPass())">
                <i class="bi" [class.bi-eye]="!showPass()" [class.bi-eye-slash]="showPass()"></i>
              </button>
            </div>
          </div>

          <button class="btn btn-primary w-100 py-2 fw-medium" [disabled]="loading()" (click)="login()">
            @if (loading()) {
              <span class="spinner-border spinner-border-sm me-2"></span>
            }
            Se connecter
          </button>

          <div class="text-center mt-3">
            <a href="#" class="text-muted small text-decoration-none">Mot de passe oublié ?</a>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  loading = signal(false);
  showPass = signal(false);

  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}

  login(): void {
    if (!this.email || !this.password) {
      this.toast.warning('Veuillez remplir tous les champs');
      return;
    }
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Connexion réussie !');
          this.router.navigate(['/dashboard']);
        } else {
          this.toast.error(res.message || 'Identifiants invalides');
        }
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Identifiants invalides');
        this.loading.set(false);
      }
    });
  }
}
