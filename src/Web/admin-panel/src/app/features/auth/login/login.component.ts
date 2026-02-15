import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email = '';
  password = '';
  loading = signal(false);
  showPass = signal(false);
  rememberMe = signal(false);

  constructor(
    private auth: AuthService,
    private router: Router,
    private toast: ToastService
  ) {}

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

  togglePassword(): void {
    this.showPass.update(v => !v);
  }

  toggleRemember(): void {
    this.rememberMe.update(v => !v);
  }
}