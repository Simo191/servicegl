import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastContainerComponent } from '../../shared/components/toast-container.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastContainerComponent],
  template: `
    <div class="d-flex">
      <!-- Sidebar -->
      <aside class="admin-sidebar" [class.show]="sidebarOpen()">
        <a routerLink="/dashboard" class="sidebar-brand">
          <i class="bi bi-grid-1x2-fill"></i>
          <span>MultiServices</span>
        </a>

        <div class="sidebar-section-title">Principal</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/dashboard" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-speedometer2"></i> Dashboard
          </a>
        </nav>

        <div class="sidebar-section-title">Gestion</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/users" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-people"></i> Utilisateurs
          </a>
          <a class="nav-link" routerLink="/restaurants" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-shop"></i> Restaurants
          </a>
          <a class="nav-link" routerLink="/services" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-tools"></i> Services
          </a>
          <a class="nav-link" routerLink="/grocery" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-cart3"></i> Courses
          </a>
          <a class="nav-link" routerLink="/orders" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-receipt"></i> Commandes
          </a>
          <a class="nav-link" routerLink="/delivery" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-truck"></i> Livreurs
          </a>
        </nav>

        <div class="sidebar-section-title">Finance & Marketing</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/finance" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-wallet2"></i> Finances
          </a>
          <a class="nav-link" routerLink="/marketing" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-megaphone"></i> Marketing
          </a>
        </nav>

        <div class="sidebar-section-title">Système</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/settings" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-gear"></i> Configuration
          </a>
        </nav>
      </aside>

      <!-- Main Content -->
      <main class="admin-main flex-grow-1">
        <!-- Header -->
        <header class="admin-header">
          <div class="d-flex align-items-center gap-3">
            <button class="btn btn-link d-lg-none p-0 text-dark" (click)="toggleSidebar()">
              <i class="bi bi-list fs-4"></i>
            </button>
            <div class="input-group" style="max-width:320px">
              <span class="input-group-text bg-light border-end-0"><i class="bi bi-search text-muted"></i></span>
              <input type="text" class="form-control bg-light border-start-0" placeholder="Rechercher...">
            </div>
          </div>
          <div class="d-flex align-items-center gap-3">
            <button class="btn btn-link position-relative text-dark">
              <i class="bi bi-bell fs-5"></i>
              <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" style="font-size:0.65rem">3</span>
            </button>
            <div class="dropdown">
              <button class="btn btn-link text-dark dropdown-toggle d-flex align-items-center gap-2 text-decoration-none" data-bs-toggle="dropdown">
                <div class="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center" style="width:36px;height:36px;font-size:0.85rem">
                  {{ userInitials() }}
                </div>
                <span class="d-none d-md-inline">{{ auth.user()?.firstName }}</span>
              </button>
              <ul class="dropdown-menu dropdown-menu-end">
                <li><a class="dropdown-item" href="#"><i class="bi bi-person me-2"></i>Mon profil</a></li>
                <li><hr class="dropdown-divider"></li>
                <li><a class="dropdown-item text-danger" (click)="auth.logout()" style="cursor:pointer"><i class="bi bi-box-arrow-right me-2"></i>Déconnexion</a></li>
              </ul>
            </div>
          </div>
        </header>

        <!-- Page Content -->
        <div class="admin-content">
          <router-outlet />
        </div>
      </main>
    </div>

    <app-toast-container />

    @if (sidebarOpen()) {
      <div class="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-50 d-lg-none" style="z-index:1025" (click)="closeMobile()"></div>
    }
  `
})
export class AdminLayoutComponent {
  sidebarOpen = signal(false);

  constructor(public auth: AuthService) {}

  userInitials(): string {
    const u = this.auth.user();
    return u ? (u.firstName[0] + u.lastName[0]).toUpperCase() : 'AD';
  }

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  closeMobile(): void {
    this.sidebarOpen.set(false);
  }
}
