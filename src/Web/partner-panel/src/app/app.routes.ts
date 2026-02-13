import { Routes } from '@angular/router';
import { PartnerLayoutComponent } from './layouts/partner-layout/partner-layout.component';

export const routes: Routes = [
  { path: 'auth', loadComponent: () => import('./features/auth/login.component').then(m => m.PartnerLoginComponent) },
  {
    path: '', component: PartnerLayoutComponent, children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/partner-dashboard.component').then(m => m.PartnerDashboardComponent) },
      { path: 'orders', loadComponent: () => import('./features/orders/partner-orders.component').then(m => m.PartnerOrdersComponent) },
      { path: 'menu', loadComponent: () => import('./features/menu/menu-management.component').then(m => m.MenuManagementComponent) },
      { path: 'services', loadComponent: () => import('./features/services-mgmt/services-management.component').then(m => m.ServicesManagementComponent) },
      { path: 'catalog', loadComponent: () => import('./features/catalog/catalog-management.component').then(m => m.CatalogManagementComponent) },
      { path: 'stock', loadComponent: () => import('./features/stock/stock-management.component').then(m => m.StockManagementComponent) },
      { path: 'availability', loadComponent: () => import('./features/availability/availability.component').then(m => m.AvailabilityComponent) },
      { path: 'team', loadComponent: () => import('./features/team/team-management.component').then(m => m.TeamManagementComponent) },
      { path: 'finances', loadComponent: () => import('./features/finances/partner-finances.component').then(m => m.PartnerFinancesComponent) },
      { path: 'profile', loadComponent: () => import('./features/profile/partner-profile.component').then(m => m.PartnerProfileComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
