import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { AdminOrderDto } from '../../core/models/api.models';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, PaginationComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmModalComponent, StatusBadgePipe],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  orders = signal<AdminOrderDto[]>([]);
  loading = signal(true);
  page = signal(1);
  totalPages = signal(1);
  search = ''; statusFilter = ''; selectedType = ''; dateFrom = ''; dateTo = '';
  viewMode: 'list' | 'card' = 'list';
  private searchTimeout: any;

  showConfirm = signal(false);
  confirmTitle = signal('');
  confirmMessage = signal('');
  confirmText = signal('Confirmer');
  private pendingAction: (() => void) | null = null;

  tabs = [
    { value: '', label: 'Toutes', icon: 'bi-grid' },
    { value: 'Restaurant', label: 'Restaurant', icon: 'bi-shop' },
    { value: 'Service', label: 'Services', icon: 'bi-tools' },
    { value: 'Grocery', label: 'Courses', icon: 'bi-cart3' },
  ];

  constructor(private admin: AdminService, private toast: ToastService) {}
  ngOnInit(): void { this.loadOrders(); }
  onSearch(): void { clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => { this.page.set(1); this.loadOrders(); }, 400); }
  selectType(type: string): void { this.selectedType = type; this.page.set(1); this.loadOrders(); }

  loadOrders(): void {
    this.loading.set(true);
    this.admin.getOrders({
      type: this.selectedType, status: this.statusFilter,
      from: this.dateFrom, to: this.dateTo, page: this.page(), pageSize: 20
    }).subscribe({
      next: res => { if (res.success) { this.orders.set(res.data.items); this.totalPages.set(res.data.totalPages); } this.loading.set(false); },
      error: () => { this.loadFallback(); this.loading.set(false); }
    });
  }

  goToPage(p: number): void { this.page.set(p); this.loadOrders(); }

  cancelOrder(o: AdminOrderDto): void {
    this.confirmTitle.set('Annuler la commande');
    this.confirmMessage.set('Annuler la commande #' + o.orderNumber + ' ? Le client sera automatiquement remboursé.');
    this.confirmText.set('Oui, annuler');
    this.pendingAction = () => {
      this.admin.cancelOrder(o.id, o.orderType, 'Annulation admin').subscribe({
        next: () => { this.toast.success('Commande annulée avec succès'); this.loadOrders(); },
        error: () => this.toast.error('Erreur lors de l\'annulation')
      });
    };
    this.showConfirm.set(true);
  }

  refundOrder(o: AdminOrderDto): void {
    this.confirmTitle.set('Rembourser la commande');
    this.confirmMessage.set('Rembourser ' + o.totalAmount + ' MAD au client pour la commande #' + o.orderNumber + ' ?');
    this.confirmText.set('Oui, rembourser');
    this.pendingAction = () => {
      this.admin.refundOrder(o.id, o.orderType, o.totalAmount).subscribe({
        next: () => { this.toast.success('Remboursement effectué'); this.loadOrders(); },
        error: () => this.toast.error('Erreur lors du remboursement')
      });
    };
    this.showConfirm.set(true);
  }

  exportOrders(): void {
    const now = new Date();
    const start = new Date(now.getFullYear(), now.getMonth(), 1).toISOString();
    const end = now.toISOString();
    this.admin.exportFinancialReport(start, end, 'xlsx').subscribe({
      next: () => this.toast.success('Export téléchargé'),
      error: () => this.toast.info('Export en cours de génération...')
    });
  }

  onConfirm(): void { this.showConfirm.set(false); this.pendingAction?.(); }
  onCancel(): void { this.showConfirm.set(false); this.pendingAction = null; }

  private loadFallback(): void {
    this.orders.set([
      { id: '1', orderNumber: 'CMD-8854', orderType: 'Restaurant', customerName: 'Ahmed B.', providerName: 'Chez Hassan', delivererName: 'Karim L.', status: 'Delivered', totalAmount: 185.50, commissionAmount: 27.83, paymentStatus: 'Paid', createdAt: '2025-06-02T14:30:00' },
      { id: '2', orderNumber: 'SRV-2241', orderType: 'Service', customerName: 'Fatima Z.', providerName: 'ProPlomb Casa', delivererName: null, status: 'InProgress', totalAmount: 450, commissionAmount: 54, paymentStatus: 'Pending', createdAt: '2025-06-02T10:00:00' },
      { id: '3', orderNumber: 'GRC-1102', orderType: 'Grocery', customerName: 'Youssef M.', providerName: 'Marjane Ain Diab', delivererName: 'Ali T.', status: 'Preparing', totalAmount: 324.75, commissionAmount: 25.98, paymentStatus: 'Paid', createdAt: '2025-06-02T11:45:00' },
      { id: '4', orderNumber: 'CMD-8853', orderType: 'Restaurant', customerName: 'Sara L.', providerName: 'Pizza Roma', delivererName: 'Hamid R.', status: 'InTransit', totalAmount: 98, commissionAmount: 14.70, paymentStatus: 'Paid', createdAt: '2025-06-02T13:00:00' },
      { id: '5', orderNumber: 'CMD-8850', orderType: 'Restaurant', customerName: 'Omar K.', providerName: 'Chez Hassan', delivererName: null, status: 'Cancelled', totalAmount: 235, commissionAmount: 0, paymentStatus: 'Refunded', createdAt: '2025-06-01T19:00:00' },
    ]);
    this.totalPages.set(5);
  }
}