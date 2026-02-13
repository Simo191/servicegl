import { Component, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-partner-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="d-flex">
      <aside class="admin-sidebar" [class.show]="sidebarOpen()">
        <a routerLink="/dashboard" class="sidebar-brand">
          <i class="bi bi-building"></i>
          <span>{{ auth.currentUser()?.businessName || 'Partenaire' }}</span>
        </a>

        <div class="sidebar-section-title">Navigation</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/dashboard" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-speedometer2"></i> Dashboard
          </a>
          <a class="nav-link" routerLink="/orders" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-receipt"></i> Commandes
          </a>

          @if (isRestaurant()) {
            <a class="nav-link" routerLink="/menu" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-menu-button-wide"></i> Menu
            </a>
          }
          @if (isService()) {
            <a class="nav-link" routerLink="/services" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-tools"></i> Services
            </a>
            <a class="nav-link" routerLink="/availability" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-calendar3"></i> Disponibilités
            </a>
            <a class="nav-link" routerLink="/team" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-people"></i> Équipe
            </a>
          }
          @if (isGrocery()) {
            <a class="nav-link" routerLink="/catalog" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-box-seam"></i> Catalogue
            </a>
            <a class="nav-link" routerLink="/stock" routerLinkActive="active" (click)="closeMobile()">
              <i class="bi bi-clipboard-data"></i> Stock
            </a>
          }
        </nav>

        <div class="sidebar-section-title">Compte</div>
        <nav class="nav flex-column">
          <a class="nav-link" routerLink="/finances" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-wallet2"></i> Finances
          </a>
          <a class="nav-link" routerLink="/profile" routerLinkActive="active" (click)="closeMobile()">
            <i class="bi bi-gear"></i> Profil
          </a>
          <a class="nav-link text-danger" (click)="auth.logout()" style="cursor:pointer">
            <i class="bi bi-box-arrow-right"></i> Déconnexion
          </a>
        </nav>
      </aside>

      <main class="admin-main flex-grow-1">
        <header class="admin-header">
          <button class="btn btn-link d-lg-none p-0 text-dark" (click)="toggleSidebar()"><i class="bi bi-list fs-4"></i></button>
          <div class="d-flex align-items-center gap-2">
            <span class="badge" [class]="typeClass()">{{ typeLabel() }}</span>
            <span class="fw-medium d-none d-md-inline">{{ auth.currentUser()?.name }}</span>
          </div>
        </header>
        <div class="admin-content"><router-outlet /></div>
      </main>
    </div>

    @if (sidebarOpen()) {
      <div class="position-fixed top-0 start-0 w-100 h-100 bg-dark bg-opacity-50 d-lg-none" style="z-index:1025" (click)="closeMobile()"></div>
    }
  `
})
export class PartnerLayoutComponent {
  sidebarOpen = signal(false);
  constructor(public auth: AuthService) {}

  isRestaurant = computed(() => this.auth.businessType() === 'Restaurant');
  isService = computed(() => this.auth.businessType() === 'ServiceProvider');
  isGrocery = computed(() => this.auth.businessType() === 'GroceryStore');
  typeClass = computed(() => { const t = this.auth.businessType(); return t === 'Restaurant' ? 'bg-warning' : t === 'ServiceProvider' ? 'bg-info' : 'bg-success'; });
  typeLabel = computed(() => { const t = this.auth.businessType(); return t === 'Restaurant' ? '🍔 Restaurant' : t === 'ServiceProvider' ? '🛠️ Services' : '🛒 Magasin'; });

  toggleSidebar() { this.sidebarOpen.update(v => !v); }
  closeMobile() { this.sidebarOpen.set(false); }
}
