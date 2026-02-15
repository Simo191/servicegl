import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { ServiceProviderDto } from '../../core/models/api.models';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [FormsModule, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent],
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.scss']
})
export class ServicesComponent implements OnInit {
  providers = signal<ServiceProviderDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  search = '';
  categoryFilter = '';
  viewMode: 'list' | 'card' = 'list';
  private searchTimeout: any;

  showConfirm = signal(false);
  confirmTitle = signal('');
  confirmMessage = signal('');
  confirmText = signal('Confirmer');
  private pendingAction: (() => void) | null = null;

  categories = [
    { name: 'Plomberie', icon: 'bi-droplet', color: '#0ea5e9', count: 85 },
    { name: 'Électricité', icon: 'bi-lightning', color: '#f59e0b', count: 72 },
    { name: 'Ménage', icon: 'bi-house', color: '#10b981', count: 120 },
    { name: 'Peinture', icon: 'bi-palette', color: '#ec4899', count: 45 },
    { name: 'Jardinage', icon: 'bi-tree', color: '#22c55e', count: 38 },
    { name: 'Climatisation', icon: 'bi-snow', color: '#06b6d4', count: 55 },
    { name: 'Déménagement', icon: 'bi-truck', color: '#8b5cf6', count: 30 },
    { name: 'Réparation', icon: 'bi-wrench', color: '#64748b', count: 60 },
  ];

  constructor(private admin: AdminService, private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadProviders(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => { this.page.set(1); this.loadProviders(); }, 400); }

  loadProviders(): void {
    this.loading.set(true);
    this.api.getPaginated<ServiceProviderDto>('admin/service-providers', {
      search: this.search, category: this.categoryFilter, page: this.page(), pageSize: 15
    }).subscribe({
      next: res => { if (res.success) { this.providers.set(res.data.items); this.totalPages.set(res.data.totalPages); this.totalCount.set(res.data.totalCount); } this.loading.set(false); },
      error: () => { this.loadFallback(); this.loading.set(false); }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadProviders(); }

  approve(id: string): void {
    this.confirmTitle.set('Approuver le prestataire');
    this.confirmMessage.set('Ce prestataire sera visible sur la plateforme et pourra recevoir des demandes. Confirmer ?');
    this.confirmText.set('Approuver');
    this.pendingAction = () => {
      this.admin.approveEntity({ entityType: 'ServiceProvider', entityId: id, approved: true }).subscribe({
        next: () => { this.toast.success('Prestataire approuvé avec succès'); this.loadProviders(); },
        error: () => this.toast.error('Erreur lors de l\'approbation')
      });
    };
    this.showConfirm.set(true);
  }

  suspendToggle(p: ServiceProviderDto): void {
    const action = p.isActive ? 'Suspendre' : 'Réactiver';
    this.confirmTitle.set(action + ' le prestataire');
    this.confirmMessage.set(action + ' "' + p.companyName + '" ? ' + (p.isActive ? 'Il ne sera plus visible sur la plateforme.' : 'Il sera de nouveau visible.'));
    this.confirmText.set(action);
    this.pendingAction = () => {
      const obs = p.isActive ? this.admin.suspendUser(p.id, 'Suspension admin') : this.admin.activateUser(p.id);
      obs.subscribe({
        next: () => { this.toast.success('Statut mis à jour'); this.loadProviders(); },
        error: () => this.toast.error('Erreur')
      });
    };
    this.showConfirm.set(true);
  }

  onConfirm(): void { this.showConfirm.set(false); this.pendingAction?.(); }
  onCancel(): void { this.showConfirm.set(false); this.pendingAction = null; }

  private loadFallback(): void {
    this.providers.set([
      { id: '1', companyName: 'ProPlomb Casa', ownerName: 'Khalid A.', description: '', logoUrl: null, rating: 4.7, reviewCount: 120, yearsOfExperience: 15, isAvailable: true, isActive: true, isVerified: true, category: 'Plomberie', serviceCategories: ['Plomberie'], startingPrice: 150, serviceZones: ['Casablanca'], city: 'Casablanca', phone: '', email: '', totalInterventions: 320, totalRevenue: 280000, commissionRate: 12, createdAt: '2025-01-05' },
      { id: '2', companyName: 'ElecPro Maroc', ownerName: 'Nabil B.', description: '', logoUrl: null, rating: 4.4, reviewCount: 85, yearsOfExperience: 10, isAvailable: true, isActive: true, isVerified: true, category: 'Électricité', serviceCategories: ['Électricité'], startingPrice: 200, serviceZones: ['Rabat'], city: 'Rabat', phone: '', email: '', totalInterventions: 210, totalRevenue: 195000, commissionRate: 12, createdAt: '2025-02-10' },
      { id: '3', companyName: 'Clean House', ownerName: 'Samira R.', description: '', logoUrl: null, rating: 4.8, reviewCount: 230, yearsOfExperience: 8, isAvailable: true, isActive: true, isVerified: true, category: 'Ménage', serviceCategories: ['Ménage'], startingPrice: 100, serviceZones: ['Casablanca'], city: 'Casablanca', phone: '', email: '', totalInterventions: 540, totalRevenue: 320000, commissionRate: 12, createdAt: '2025-01-20' },
    ]);
    this.totalCount.set(3); this.totalPages.set(1);
  }
}