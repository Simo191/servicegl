import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { UserDto, PaginatedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="fw-bold mb-1">Gestion des Utilisateurs</h4>
        <p class="text-muted mb-0">{{ totalCount() }} utilisateurs inscrits</p>
      </div>
      <button class="btn btn-primary" (click)="exportUsers()">
        <i class="bi bi-download me-2"></i>Exporter
      </button>
    </div>

    <!-- Filters -->
    <div class="table-card mb-4">
      <div class="card-body p-3">
        <div class="row g-3 align-items-end">
          <div class="col-md-4">
            <label class="form-label small fw-medium">Recherche</label>
            <div class="input-group">
              <span class="input-group-text"><i class="bi bi-search"></i></span>
              <input type="text" class="form-control" placeholder="Nom, email, téléphone..." [(ngModel)]="search" (input)="onSearch()" />
            </div>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Rôle</label>
            <select class="form-select" [(ngModel)]="roleFilter" (change)="loadUsers()">
              <option value="">Tous</option>
              <option value="Client">Client</option>
              <option value="RestaurantOwner">Restaurant</option>
              <option value="ServiceProvider">Prestataire</option>
              <option value="GroceryManager">Magasin</option>
              <option value="Deliverer">Livreur</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Statut</label>
            <select class="form-select" [(ngModel)]="statusFilter" (change)="loadUsers()">
              <option value="">Tous</option>
              <option value="true">Actif</option>
              <option value="false">Inactif</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label small fw-medium">Tri</label>
            <select class="form-select" [(ngModel)]="sortBy" (change)="loadUsers()">
              <option value="createdAt">Date inscription</option>
              <option value="lastLoginAt">Dernière connexion</option>
              <option value="totalSpent">Total dépensé</option>
            </select>
          </div>
          <div class="col-md-2">
            <button class="btn btn-outline-secondary w-100" (click)="resetFilters()">
              <i class="bi bi-x-circle me-1"></i>Réinitialiser
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Table -->
    @if (loading()) {
      <app-loading />
    } @else if (users().length === 0) {
      <app-empty-state icon="bi-people" message="Aucun utilisateur trouvé" />
    } @else {
      <div class="table-card">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>Utilisateur</th>
                <th>Rôle</th>
                <th>Téléphone</th>
                <th>Statut</th>
                <th>Commandes</th>
                <th>Dépensé</th>
                <th>Inscrit le</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (user of users(); track user.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="rounded-circle bg-primary bg-opacity-10 text-primary d-flex align-items-center justify-content-center" style="width:36px;height:36px;font-size:0.8rem;font-weight:600">
                        {{ user.firstName[0] }}{{ user.lastName[0] }}
                      </div>
                      <div>
                        <div class="fw-medium">{{ user.firstName }} {{ user.lastName }}</div>
                        <small class="text-muted">{{ user.email }}</small>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span class="badge bg-primary-subtle text-primary">{{ user.role }}</span>
                  </td>
                  <td>{{ user.phoneNumber || '-' }}</td>
                  <td>
                    @if (user.isActive) {
                      <span class="badge-status bg-success-subtle text-success">Actif</span>
                    } @else {
                      <span class="badge-status bg-danger-subtle text-danger">Inactif</span>
                    }
                  </td>
                  <td>{{ user.totalOrders }}</td>
                  <td>{{ user.totalSpent | number:'1.0-0' }} MAD</td>
                  <td class="text-muted">{{ user.createdAt | date:'dd/MM/yyyy' }}</td>
                  <td class="text-end">
                    <div class="dropdown">
                      <button class="btn btn-sm btn-light" data-bs-toggle="dropdown">
                        <i class="bi bi-three-dots-vertical"></i>
                      </button>
                      <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#"><i class="bi bi-eye me-2"></i>Voir profil</a></li>
                        <li>
                          @if (user.isActive) {
                            <a class="dropdown-item text-warning" (click)="toggleUser(user)" style="cursor:pointer">
                              <i class="bi bi-pause-circle me-2"></i>Suspendre
                            </a>
                          } @else {
                            <a class="dropdown-item text-success" (click)="toggleUser(user)" style="cursor:pointer">
                              <i class="bi bi-play-circle me-2"></i>Activer
                            </a>
                          }
                        </li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item text-danger" (click)="prepareDelete(user)" style="cursor:pointer"><i class="bi bi-trash me-2"></i>Supprimer</a></li>
                      </ul>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
        <div class="p-3 border-top">
          <app-pagination [currentPage]="page()" [totalPages]="totalPages()" (pageChange)="goToPage($event)" />
        </div>
      </div>
    }

    <app-confirm-modal [show]="showDeleteModal()" title="Supprimer l'utilisateur"
      [message]="'Êtes-vous sûr de vouloir supprimer ' + (selectedUser()?.firstName || '') + ' ' + (selectedUser()?.lastName || '') + ' ?'"
      confirmText="Supprimer" (confirmed)="deleteUser()" (cancelled)="showDeleteModal.set(false)" />
  `
})
export class UsersComponent implements OnInit {
  users = signal<UserDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);

  search = '';
  roleFilter = '';
  statusFilter = '';
  sortBy = 'createdAt';

  showDeleteModal = signal(false);
  selectedUser = signal<UserDto | null>(null);

  private searchTimeout: any;

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => this.loadUsers(), 400);
  }

  loadUsers(): void {
    this.loading.set(true);
    this.api.getPaginated<UserDto>('admin/users', {
      search: this.search, role: this.roleFilter, isActive: this.statusFilter,
      sortBy: this.sortBy, page: this.page(), pageSize: 15
    }).subscribe({
      next: res => {
        if (res.success) {
          this.users.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.totalCount.set(res.data.totalCount);
        }
        this.loading.set(false);
      },
      error: () => {
        // Demo
        this.users.set([
          { id: '1', firstName: 'Ahmed', lastName: 'Bennani', email: 'ahmed@mail.com', phoneNumber: '+212612345678', photoUrl: '', role: 'Client', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-01-15', lastLoginAt: '2025-06-01', totalOrders: 45, totalSpent: 12500 },
          { id: '2', firstName: 'Fatima', lastName: 'Zahra', email: 'fatima@mail.com', phoneNumber: '+212698765432', photoUrl: '', role: 'Client', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-02-20', lastLoginAt: '2025-06-02', totalOrders: 32, totalSpent: 8900 },
          { id: '3', firstName: 'Youssef', lastName: 'Alami', email: 'youssef@restaurant.ma', phoneNumber: '+212655555555', photoUrl: '', role: 'RestaurantOwner', isActive: true, emailVerified: true, phoneVerified: true, createdAt: '2025-01-10', lastLoginAt: '2025-06-02', totalOrders: 0, totalSpent: 0 },
          { id: '4', firstName: 'Omar', lastName: 'Tazi', email: 'omar@services.ma', phoneNumber: '+212644444444', photoUrl: '', role: 'ServiceProvider', isActive: false, emailVerified: true, phoneVerified: false, createdAt: '2025-03-05', lastLoginAt: '2025-05-20', totalOrders: 0, totalSpent: 0 },
        ]);
        this.totalCount.set(4);
        this.totalPages.set(1);
        this.loading.set(false);
      }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadUsers(); }
  resetFilters(): void { this.search = ''; this.roleFilter = ''; this.statusFilter = ''; this.sortBy = 'createdAt'; this.loadUsers(); }
  exportUsers(): void { this.toast.info('Export en cours...'); }

  toggleUser(user: UserDto): void {
    this.api.patch(`admin/users/${user.id}/toggle`, {}).subscribe({
      next: () => { this.toast.success('Statut mis à jour'); this.loadUsers(); },
      error: () => this.toast.error('Erreur lors de la mise à jour')
    });
  }

  prepareDelete(user: UserDto): void {
    this.selectedUser.set(user);
    this.showDeleteModal.set(true);
  }

  deleteUser(): void {
    const user = this.selectedUser();
    if (!user) return;
    this.api.delete(`admin/users/${user.id}`).subscribe({
      next: () => { this.toast.success('Utilisateur supprimé'); this.showDeleteModal.set(false); this.loadUsers(); },
      error: () => this.toast.error('Erreur lors de la suppression')
    });
  }
}
