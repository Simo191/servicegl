import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { AdminUserDto, PaginatedResult } from '../../core/models/api.models';

interface RoleOption { 
  value: string; 
  label: string; 
  icon: string; 
  color: string; 
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    FormsModule, 
    DatePipe, 
    DecimalPipe, 
    PaginationComponent, 
    LoadingSpinnerComponent, 
    EmptyStateComponent, 
    ConfirmModalComponent
  ],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  // Signals pour la réactivité
  users = signal<AdminUserDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  // Filtres et recherche
  search = '';
  roleFilter = '';
  statusFilter = '';
  sortBy = 'createdAt';
  viewMode: 'list' | 'card' = 'list';

  // Modals de confirmation
  showDeleteModal = signal(false);
  showActivateModal = signal(false);
  showSuspendModal = signal(false);
  showEditModal = signal(false);
  showAddModal = signal(false);
  
  selectedUser = signal<AdminUserDto | null>(null);
  
  // Formulaire d'édition/ajout
  userForm = {
    fullName: '',
    email: '',
    phone: '',
    role: 'Client',
    isActive: true
  };

  private searchTimeout: any;

  roleOptions: RoleOption[] = [
    { value: '', label: 'Tous les rôles', icon: 'bi-people', color: '' },
    { value: 'Client', label: 'Client', icon: 'bi-person', color: 'primary' },
    { value: 'RestaurantOwner', label: 'Restaurant', icon: 'bi-shop', color: 'warning' },
    { value: 'ServiceProvider', label: 'Prestataire', icon: 'bi-tools', color: 'info' },
    { value: 'GroceryManager', label: 'Magasin', icon: 'bi-cart3', color: 'success' },
    { value: 'Deliverer', label: 'Livreur', icon: 'bi-truck', color: 'danger' },
    { value: 'Admin', label: 'Admin', icon: 'bi-shield-lock', color: 'dark' }
  ];

  constructor(
    private api: ApiService, 
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  // ═══════════════════════════════════════════════════════════
  // RECHERCHE & FILTRES
  // ═══════════════════════════════════════════════════════════
  
  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => { 
      this.page.set(1); 
      this.loadUsers(); 
    }, 400);
  }

  resetFilters(): void {
    this.search = '';
    this.roleFilter = '';
    this.statusFilter = '';
    this.sortBy = 'createdAt';
    this.page.set(1);
    this.loadUsers();
  }

  hasActiveFilters(): boolean {
    return this.roleFilter !== '' || this.statusFilter !== '' || this.sortBy !== 'createdAt';
  }

  // ═══════════════════════════════════════════════════════════
  // CHARGEMENT DES DONNÉES (READ)
  // ═══════════════════════════════════════════════════════════
  
  loadUsers(): void {
    this.loading.set(true);
    this.api.getPaginated<AdminUserDto>('admin/users', {
      search: this.search,
      role: this.roleFilter,
      isActive: this.statusFilter,
      sortBy: this.sortBy,
      page: this.page(),
      pageSize: 15
    }).subscribe({
      next: res => {
        if (res.success) {
          this.users.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.totalCount.set(res.data.totalCount);
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.toast.error('Erreur lors du chargement des utilisateurs');
        this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { 
    this.page.set(p); 
    this.loadUsers(); 
  }

  // ═══════════════════════════════════════════════════════════
  // CRÉATION (CREATE)
  // ═══════════════════════════════════════════════════════════
  
  openAddModal(): void {
    this.userForm = {
      fullName: '',
      email: '',
      phone: '',
      role: 'Client',
      isActive: true
    };
    this.showAddModal.set(true);
  }

  createUser(): void {
    if (!this.userForm.fullName || !this.userForm.email) {
      this.toast.warning('Veuillez remplir tous les champs obligatoires');
      return;
    }

    this.api.post('admin/users', this.userForm).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Utilisateur créé avec succès');
          this.showAddModal.set(false);
          this.loadUsers();
        }
      },
      error: (err) => {
        this.toast.error('Erreur lors de la création de l\'utilisateur');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // MODIFICATION (UPDATE)
  // ═══════════════════════════════════════════════════════════
  
  openEditModal(user: AdminUserDto): void {
    this.selectedUser.set(user);
    this.userForm = {
      fullName: user.fullName,
      email: user.email,
      phone: user.phone || '',
      role: user.roles[0] || 'Client',
      isActive: user.isActive
    };
    this.showEditModal.set(true);
  }

  updateUser(): void {
    const user = this.selectedUser();
    if (!user) return;

    this.api.put(`admin/users/${user.id}`, this.userForm).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Utilisateur mis à jour avec succès');
          this.showEditModal.set(false);
          this.loadUsers();
        }
      },
      error: (err) => {
        this.toast.error('Erreur lors de la mise à jour');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // ACTIVATION / DÉSACTIVATION (ACTIVATE / DEACTIVATE)
  // ═══════════════════════════════════════════════════════════
  
  prepareActivate(user: AdminUserDto): void {
    this.selectedUser.set(user);
    this.showActivateModal.set(true);
  }

  activateUser(): void {
    const user = this.selectedUser();
    if (!user) return;

    this.api.post(`admin/users/${user.id}/activate`, {}).subscribe({
      next: () => {
        this.toast.success('Utilisateur activé avec succès');
        this.showActivateModal.set(false);
        this.loadUsers();
      },
      error: (err) => {
        this.toast.error('Erreur lors de l\'activation');
      }
    });
  }

  prepareSuspend(user: AdminUserDto): void {
    this.selectedUser.set(user);
    this.showSuspendModal.set(true);
  }

  suspendUser(): void {
    const user = this.selectedUser();
    if (!user) return;

    const reason = "Suspension temporaire par l'administrateur";
    
    this.api.postJson(`admin/users/${user.id}/suspend`, JSON.stringify(reason))
      .subscribe({
        next: () => {
          this.toast.success('Utilisateur suspendu');
          this.showSuspendModal.set(false);
          this.loadUsers();
        },
        error: (err) => {
          this.toast.error('Erreur lors de la suspension');
        }
      });
  }

  // ═══════════════════════════════════════════════════════════
  // SUPPRESSION (DELETE)
  // ═══════════════════════════════════════════════════════════
  
  prepareDelete(user: AdminUserDto): void {
    this.selectedUser.set(user);
    this.showDeleteModal.set(true);
  }

  deleteUser(): void {
    const user = this.selectedUser();
    if (!user) return;

    this.api.delete(`admin/users/${user.id}`).subscribe({
      next: () => {
        this.toast.success('Utilisateur supprimé avec succès');
        this.showDeleteModal.set(false);
        this.loadUsers();
      },
      error: () => {
        this.toast.error('Erreur lors de la suppression');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // EXPORT
  // ═══════════════════════════════════════════════════════════
  
  exportUsers(): void {
    this.api.get('admin/users/export', {
      format: 'xlsx',
      search: this.search,
      role: this.roleFilter,
      isActive: this.statusFilter
    }).subscribe({
      next: () => {
        this.toast.success('Export téléchargé avec succès');
      },
      error: () => {
        this.toast.info('Export en cours de génération...');
      }
    });
  }

  // ═══════════════════════════════════════════════════════════
  // HELPERS
  // ═══════════════════════════════════════════════════════════
  
  getRoleBadge(role: string): RoleOption {
    return this.roleOptions.find(r => r.value === role) ?? this.roleOptions[0];
  }

  getInitials(user: AdminUserDto): string {
    const fullName = (user?.fullName ?? '').trim();
    if (!fullName) return '?';

    const parts = fullName.split(/\s+/).filter(Boolean);

    if (parts.length === 1) {
      return parts[0].substring(0, 2).toUpperCase();
    }

    const first = parts[0][0] ?? '';
    const last = parts[parts.length - 1][0] ?? '';
    return (first + last).toUpperCase();
  }

  getAvatarColor(user: AdminUserDto): string {
    const colors = [
      '#6366f1', '#f59e0b', '#10b981', '#ef4444', 
      '#06b6d4', '#8b5cf6', '#ec4899'
    ];

    const initials = this.getInitials(user);
    let hash = 0;
    for (let i = 0; i < initials.length; i++) {
      hash = (hash * 31 + initials.charCodeAt(i)) | 0;
    }

    return colors[Math.abs(hash) % colors.length];
  }
}