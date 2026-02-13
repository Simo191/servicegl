#!/bin/bash
set -e
BASE="/home/claude/MultiServicesApp/src/Web/admin-panel/src/app"

# ============================================================
# ADMIN LAYOUT
# ============================================================
mkdir -p "$BASE/layouts/admin-layout"

cat > "$BASE/layouts/admin-layout/admin-layout.component.ts" << 'TSEOF'
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
TSEOF

# ============================================================
# SHARED COMPONENTS
# ============================================================
mkdir -p "$BASE/shared/components"

cat > "$BASE/shared/components/toast-container.component.ts" << 'TSEOF'
import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="position-fixed bottom-0 end-0 p-3" style="z-index:9999">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast show mb-2 border-0 shadow"
             [class.bg-success]="toast.type === 'success'"
             [class.bg-danger]="toast.type === 'error'"
             [class.bg-warning]="toast.type === 'warning'"
             [class.bg-info]="toast.type === 'info'"
             [class.text-white]="toast.type !== 'warning'">
          <div class="toast-body d-flex align-items-center justify-content-between">
            <span>{{ toast.message }}</span>
            <button class="btn-close btn-close-white ms-2" (click)="toastService.remove(toast.id)"></button>
          </div>
        </div>
      }
    </div>
  `
})
export class ToastContainerComponent {
  toastService = inject(ToastService);
}
TSEOF

cat > "$BASE/shared/components/pagination.component.ts" << 'TSEOF'
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  template: `
    @if (totalPages() > 1) {
      <nav>
        <ul class="pagination pagination-sm justify-content-center mb-0">
          <li class="page-item" [class.disabled]="currentPage() === 1">
            <a class="page-link" (click)="pageChange.emit(currentPage() - 1)" style="cursor:pointer">
              <i class="bi bi-chevron-left"></i>
            </a>
          </li>
          @for (page of pages(); track page) {
            <li class="page-item" [class.active]="page === currentPage()">
              <a class="page-link" (click)="pageChange.emit(page)" style="cursor:pointer">{{ page }}</a>
            </li>
          }
          <li class="page-item" [class.disabled]="currentPage() === totalPages()">
            <a class="page-link" (click)="pageChange.emit(currentPage() + 1)" style="cursor:pointer">
              <i class="bi bi-chevron-right"></i>
            </a>
          </li>
        </ul>
      </nav>
    }
  `
})
export class PaginationComponent {
  currentPage = input(1);
  totalPages = input(1);
  pageChange = output<number>();

  pages(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }
}
TSEOF

cat > "$BASE/shared/components/stat-card.component.ts" << 'TSEOF'
import { Component, input } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <div class="stat-card">
      <div class="d-flex align-items-start justify-content-between">
        <div>
          <p class="stat-label mb-1">{{ label() }}</p>
          <h3 class="stat-value mb-1">
            @if (isCurrency()) {
              {{ value() | number:'1.0-0' }} <small class="fs-6 fw-normal">MAD</small>
            } @else {
              {{ value() | number:'1.0-0' }}
            }
          </h3>
          @if (change() !== null) {
            <span class="stat-change" [class.text-success]="change()! >= 0" [class.text-danger]="change()! < 0">
              <i class="bi" [class.bi-arrow-up]="change()! >= 0" [class.bi-arrow-down]="change()! < 0"></i>
              {{ change()! >= 0 ? '+' : '' }}{{ change() }}%
              <span class="text-muted fw-normal"> vs mois dernier</span>
            </span>
          }
        </div>
        <div class="stat-icon" [style.background-color]="bgColor()">
          <i class="bi" [class]="icon()" [style.color]="iconColor()"></i>
        </div>
      </div>
    </div>
  `
})
export class StatCardComponent {
  label = input('');
  value = input(0);
  change = input<number | null>(null);
  icon = input('bi-bar-chart');
  bgColor = input('#eef2ff');
  iconColor = input('#4f46e5');
  isCurrency = input(false);
}
TSEOF

cat > "$BASE/shared/components/confirm-modal.component.ts" << 'TSEOF'
import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  template: `
    @if (show()) {
      <div class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,0.5)">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content border-0 shadow">
            <div class="modal-header border-0">
              <h5 class="modal-title">{{ title() }}</h5>
              <button class="btn-close" (click)="cancelled.emit()"></button>
            </div>
            <div class="modal-body">
              <p class="text-muted mb-0">{{ message() }}</p>
            </div>
            <div class="modal-footer border-0">
              <button class="btn btn-light" (click)="cancelled.emit()">Annuler</button>
              <button class="btn" [class]="'btn-' + confirmColor()" (click)="confirmed.emit()">
                {{ confirmText() }}
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class ConfirmModalComponent {
  show = input(false);
  title = input('Confirmation');
  message = input('Êtes-vous sûr ?');
  confirmText = input('Confirmer');
  confirmColor = input('danger');
  confirmed = output<void>();
  cancelled = output<void>();
}
TSEOF

cat > "$BASE/shared/components/loading-spinner.component.ts" << 'TSEOF'
import { Component } from '@angular/core';

@Component({
  selector: 'app-loading',
  standalone: true,
  template: `
    <div class="text-center p-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Chargement...</span>
      </div>
      <p class="text-muted mt-2 mb-0">Chargement...</p>
    </div>
  `
})
export class LoadingSpinnerComponent {}
TSEOF

cat > "$BASE/shared/components/empty-state.component.ts" << 'TSEOF'
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="text-center p-5">
      <i class="bi" [class]="icon()" style="font-size:3rem;color:#cbd5e1"></i>
      <p class="text-muted mt-2 mb-0">{{ message() }}</p>
    </div>
  `
})
export class EmptyStateComponent {
  icon = input('bi-inbox');
  message = input('Aucune donnée trouvée');
}
TSEOF

# ============================================================
# SHARED PIPES
# ============================================================
mkdir -p "$BASE/shared/pipes"

cat > "$BASE/shared/pipes/status-badge.pipe.ts" << 'TSEOF'
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'statusBadge', standalone: true })
export class StatusBadgePipe implements PipeTransform {
  private colors: Record<string, string> = {
    'Pending': 'warning', 'Confirmed': 'info', 'Preparing': 'primary',
    'Ready': 'secondary', 'InTransit': 'info', 'Delivered': 'success',
    'Completed': 'success', 'Cancelled': 'danger', 'Refunded': 'dark',
    'Active': 'success', 'Inactive': 'secondary', 'Verified': 'success',
    'Unverified': 'warning', 'Online': 'success', 'Offline': 'secondary',
    'Paid': 'success', 'Unpaid': 'danger', 'Processing': 'info',
    'Reserved': 'warning', 'EnRoute': 'info', 'OnSite': 'primary',
    'InProgress': 'primary'
  };

  private labels: Record<string, string> = {
    'Pending': 'En attente', 'Confirmed': 'Confirmée', 'Preparing': 'En préparation',
    'Ready': 'Prête', 'InTransit': 'En route', 'Delivered': 'Livrée',
    'Completed': 'Terminée', 'Cancelled': 'Annulée', 'Refunded': 'Remboursée',
    'Active': 'Actif', 'Inactive': 'Inactif', 'Verified': 'Vérifié',
    'Unverified': 'Non vérifié', 'Online': 'En ligne', 'Offline': 'Hors ligne',
    'Paid': 'Payé', 'Unpaid': 'Impayé', 'Processing': 'En cours',
    'Reserved': 'Réservée', 'EnRoute': 'En route', 'OnSite': 'Sur place',
    'InProgress': 'En cours'
  };

  transform(value: string): { label: string; color: string } {
    return {
      label: this.labels[value] || value,
      color: this.colors[value] || 'secondary'
    };
  }
}
TSEOF

echo "✅ Admin Layout + Shared components generated!"
