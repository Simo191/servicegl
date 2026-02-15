import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { GroceryStoreDto } from '../../core/models/api.models';

@Component({
  selector: 'app-grocery',
  standalone: true,
  imports: [FormsModule, DecimalPipe, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent],
  templateUrl: './grocery.component.html',
  styleUrls: ['./grocery.component.scss']
})
export class GroceryComponent implements OnInit {
  stores = signal<GroceryStoreDto[]>([]);
  loading = signal(true);
  search = '';
  selectedBrand = '';
  viewMode: 'list' | 'card' = 'list';
  currentPage = 1;
  pageSize = 8;
  private searchTimeout: any;

  showConfirm = signal(false);
  confirmTitle = signal('');
  confirmMessage = signal('');
  confirmText = signal('Confirmer');
  private pendingAction: (() => void) | null = null;

  brands = [
    { name: 'Marjane', color: '#e11d48', count: 12 },
    { name: 'Carrefour', color: '#2563eb', count: 8 },
    { name: 'Aswak Assalam', color: '#16a34a', count: 6 },
    { name: 'Acima', color: '#ea580c', count: 10 },
    { name: 'Label\'Vie', color: '#7c3aed', count: 5 },
  ];

  totalPagesCalc = computed(() => Math.ceil(this.filteredStores().length / this.pageSize) || 1);
  pages = computed(() => {
    const total = this.totalPagesCalc();
    return Array.from({ length: total }, (_, i) => i + 1);
  });

  constructor(private admin: AdminService, private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadStores(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => { this.currentPage = 1; }, 400); }
  filterByBrand(brand: string): void { this.selectedBrand = this.selectedBrand === brand ? '' : brand; this.currentPage = 1; }

  filteredStores(): GroceryStoreDto[] {
    let result = this.stores();
    if (this.selectedBrand) result = result.filter(s => s.brand === this.selectedBrand);
    if (this.search) result = result.filter(s => s.name.toLowerCase().includes(this.search.toLowerCase()));
    return result;
  }

  paginatedStores(): GroceryStoreDto[] {
    const filtered = this.filteredStores();
    const start = (this.currentPage - 1) * this.pageSize;
    return filtered.slice(start, start + this.pageSize);
  }

  loadStores(): void {
    this.loading.set(true);
    this.api.get<GroceryStoreDto[]>('admin/grocery-stores').subscribe({
      next: res => { if (res.success) this.stores.set(res.data); this.loading.set(false); },
      error: () => {
        this.stores.set([
          { id: '1', name: 'Marjane Ain Diab', brand: 'Marjane', logoUrl: null, rating: 4.3, reviewCount: 150, minOrderAmount: 50, deliveryFee: 15, freeDeliveryThreshold: 200, distanceKm: 3, isOpen: true, isActive: true, hasPromotion: true, address: 'Ain Diab', city: 'Casablanca', phone: '', totalProducts: 12500, totalOrders: 3400, totalRevenue: 1250000, commissionRate: 8, createdAt: '2025-01-01' },
          { id: '2', name: 'Carrefour Anfa', brand: 'Carrefour', logoUrl: null, rating: 4.1, reviewCount: 98, minOrderAmount: 40, deliveryFee: 12, freeDeliveryThreshold: 150, distanceKm: 5, isOpen: true, isActive: true, hasPromotion: false, address: 'Anfa Place', city: 'Casablanca', phone: '', totalProducts: 15000, totalOrders: 2800, totalRevenue: 980000, commissionRate: 8, createdAt: '2025-01-01' },
          { id: '3', name: 'Acima Maarif', brand: 'Acima', logoUrl: null, rating: 3.9, reviewCount: 67, minOrderAmount: 30, deliveryFee: 10, freeDeliveryThreshold: 100, distanceKm: 2, isOpen: true, isActive: true, hasPromotion: false, address: 'Maarif', city: 'Casablanca', phone: '', totalProducts: 8500, totalOrders: 1500, totalRevenue: 450000, commissionRate: 8, createdAt: '2025-01-01' },
        ]);
        this.loading.set(false);
      }
    });
  }

  approveStore(id: string): void {
    this.confirmTitle.set('Approuver le magasin');
    this.confirmMessage.set('Confirmer l\'approbation de ce magasin ?');
    this.confirmText.set('Approuver');
    this.pendingAction = () => {
      this.admin.approveEntity({ entityType: 'GroceryStore', entityId: id, approved: true }).subscribe({
        next: () => { this.toast.success('Magasin approuvé'); this.loadStores(); },
        error: () => this.toast.error('Erreur')
      });
    };
    this.showConfirm.set(true);
  }

  onConfirm(): void { this.showConfirm.set(false); this.pendingAction?.(); }
  onCancel(): void { this.showConfirm.set(false); this.pendingAction = null; }
}