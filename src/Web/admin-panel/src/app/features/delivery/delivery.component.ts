import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { DelivererDto } from '../../core/models/api.models';

@Component({
  selector: 'app-delivery',
  standalone: true,
  imports: [FormsModule, DecimalPipe, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent],
  templateUrl: './delivery.component.html',
  styleUrls: ['./delivery.component.scss']
})
export class DeliveryComponent implements OnInit {
  deliverers = signal<DelivererDto[]>([]);
  loading = signal(true);
  search = ''; statusFilter = ''; vehicleFilter = '';
  viewMode: 'list' | 'card' = 'list';
  currentPage = 1;
  pageSize = 8;

  showConfirm = signal(false);
  confirmTitle = signal('');
  confirmMessage = signal('');
  confirmText = signal('Confirmer');
  private pendingAction: (() => void) | null = null;

  onlineCount = computed(() => this.deliverers().filter(d => d.isOnline).length);
  offlineCount = computed(() => this.deliverers().filter(d => !d.isOnline && d.isVerified).length);
  pendingCount = computed(() => this.deliverers().filter(d => !d.isVerified).length);
  totalDeliveries = computed(() => this.deliverers().reduce((s, d) => s + d.totalDeliveries, 0));

  totalPagesCalc = computed(() => Math.ceil(this.filteredDeliverers().length / this.pageSize) || 1);
  pages = computed(() => {
    const total = this.totalPagesCalc();
    return Array.from({ length: total }, (_, i) => i + 1);
  });

  constructor(private admin: AdminService, private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadDeliverers(); }

  filteredDeliverers(): DelivererDto[] {
    let result = this.deliverers();
    if (this.search) result = result.filter(d => (d.firstName + ' ' + d.lastName).toLowerCase().includes(this.search.toLowerCase()));
    if (this.statusFilter === 'online') result = result.filter(d => d.isOnline);
    if (this.statusFilter === 'offline') result = result.filter(d => !d.isOnline && d.isVerified);
    if (this.statusFilter === 'pending') result = result.filter(d => !d.isVerified);
    if (this.vehicleFilter) result = result.filter(d => d.vehicleType === this.vehicleFilter);
    return result;
  }

  paginatedDeliverers(): DelivererDto[] {
    const filtered = this.filteredDeliverers();
    const start = (this.currentPage - 1) * this.pageSize;
    return filtered.slice(start, start + this.pageSize);
  }

  loadDeliverers(): void {
    this.loading.set(true);
    this.api.get<DelivererDto[]>('admin/deliverers').subscribe({
      next: res => { if (res.success) this.deliverers.set(res.data); this.loading.set(false); },
      error: () => {
        this.deliverers.set([
          { id: '1', name: 'Karim Lahlou', firstName: 'Karim', lastName: 'Lahlou', phone: '+212611111111', email: '', photoUrl: null, vehicleType: 'Moto', isActive: true, isOnline: true, isAvailable: true, isVerified: true, rating: 4.8, totalDeliveries: 1250, totalEarnings: 85000, latitude: 33.57, longitude: -7.59, createdAt: '2025-01-15' },
          { id: '2', name: 'Ali Tahiri', firstName: 'Ali', lastName: 'Tahiri', phone: '+212622222222', email: '', photoUrl: null, vehicleType: 'Voiture', isActive: true, isOnline: true, isAvailable: true, isVerified: true, rating: 4.5, totalDeliveries: 890, totalEarnings: 62000, latitude: 33.59, longitude: -7.61, createdAt: '2025-02-01' },
          { id: '3', name: 'Hamid Rami', firstName: 'Hamid', lastName: 'Rami', phone: '+212633333333', email: '', photoUrl: null, vehicleType: 'Scooter', isActive: true, isOnline: false, isAvailable: false, isVerified: true, rating: 4.2, totalDeliveries: 450, totalEarnings: 31000, latitude: 0, longitude: 0, createdAt: '2025-03-10' },
          { id: '4', name: 'Samir Bousfiha', firstName: 'Samir', lastName: 'Bousfiha', phone: '+212644444444', email: '', photoUrl: null, vehicleType: 'Moto', isActive: true, isOnline: false, isAvailable: false, isVerified: false, rating: 0, totalDeliveries: 0, totalEarnings: 0, latitude: 0, longitude: 0, createdAt: '2025-06-01' },
        ]);
        this.loading.set(false);
      }
    });
  }

  approve(id: string): void {
    this.confirmTitle.set('Approuver le livreur');
    this.confirmMessage.set('Ce livreur pourra commencer à livrer après approbation. Confirmer ?');
    this.confirmText.set('Approuver');
    this.pendingAction = () => {
      this.admin.approveEntity({ entityType: 'DeliveryDriver', entityId: id, approved: true }).subscribe({
        next: () => { this.toast.success('Livreur approuvé avec succès'); this.loadDeliverers(); },
        error: () => this.toast.error('Erreur')
      });
    };
    this.showConfirm.set(true);
  }

  onConfirm(): void { this.showConfirm.set(false); this.pendingAction?.(); }
  onCancel(): void { this.showConfirm.set(false); this.pendingAction = null; }
}