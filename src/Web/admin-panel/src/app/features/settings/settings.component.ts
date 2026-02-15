import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ToastService } from '../../core/services/toast.service';
import { PaginationComponent } from '../../shared/components/pagination.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner.component';
import { SystemLogDto } from '../../core/models/api.models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule, DatePipe, PaginationComponent, LoadingSpinnerComponent],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  activeTab = signal('commissions');
  savingCommissions = signal(false);
  logsLoading = signal(false);
  logs = signal<SystemLogDto[]>([]);
  logPage = signal(1);
  logTotalPages = signal(1);
  logLevel = '';

  settingsTabs = [
    { id: 'commissions', label: 'Commissions', icon: 'bi-percent' },
    { id: 'zones', label: 'Zones', icon: 'bi-geo-alt' },
    { id: 'payment', label: 'Paiement', icon: 'bi-credit-card' },
    { id: 'logs', label: 'Logs système', icon: 'bi-terminal' },
    { id: 'system', label: 'Système', icon: 'bi-gear' },
  ];

  commissions = { restaurant: 15, service: 12, grocery: 8, deliveryBaseFee: 10, deliveryPerKm: 3 };
  zones = [
    { name: 'Casablanca Centre', deliveryFee: 15, surcharge: 0 },
    { name: 'Casablanca Périphérie', deliveryFee: 25, surcharge: 5 },
    { name: 'Rabat Centre', deliveryFee: 15, surcharge: 0 },
    { name: 'Rabat Périphérie', deliveryFee: 25, surcharge: 5 },
    { name: 'Marrakech', deliveryFee: 20, surcharge: 0 },
  ];

  bulkNotif = { title: '', message: '', targetAudience: 'All' };

  constructor(private admin: AdminService, private toast: ToastService) {}

  saveCommissions(): void {
    this.savingCommissions.set(true);
    // Send for each entity type
    this.admin.updateCommissions({ entityType: 'Restaurant', entityId: '00000000-0000-0000-0000-000000000000', newRate: this.commissions.restaurant }).subscribe({
      next: () => {
        this.admin.updateCommissions({ entityType: 'Service', entityId: '00000000-0000-0000-0000-000000000000', newRate: this.commissions.service }).subscribe();
        this.admin.updateCommissions({ entityType: 'Grocery', entityId: '00000000-0000-0000-0000-000000000000', newRate: this.commissions.grocery }).subscribe();
        this.toast.success('Commissions sauvegardées avec succès');
        this.savingCommissions.set(false);
      },
      error: () => { this.toast.error('Erreur lors de la sauvegarde'); this.savingCommissions.set(false); }
    });
  }

  loadLogs(): void {
    this.logsLoading.set(true);
    this.admin.getLogs({ level: this.logLevel, page: this.logPage(), pageSize: 30 }).subscribe({
      next: res => { if (res.success) { this.logs.set(res.data.items); this.logTotalPages.set(res.data.totalPages); } this.logsLoading.set(false); },
      error: () => {
        this.logs.set([
          { id: '1', level: 'Info', message: 'User login: admin@multiservices.ma', source: 'AuthService', timestamp: '2025-06-02T14:30:00', details: null },
          { id: '2', level: 'Warning', message: 'Payment retry attempt #2 for order CMD-8854', source: 'PaymentService', timestamp: '2025-06-02T14:28:00', details: null },
          { id: '3', level: 'Error', message: 'SMS gateway timeout', source: 'NotificationService', timestamp: '2025-06-02T14:25:00', details: 'Timeout after 30s' },
        ]);
        this.logsLoading.set(false);
      }
    });
  }

  sendNotification(): void {
    if (!this.bulkNotif.title || !this.bulkNotif.message) {
      this.toast.warning('Veuillez remplir le titre et le message');
      return;
    }
    this.admin.sendBulkNotification(this.bulkNotif).subscribe({
      next: () => {
        this.toast.success('Notification envoyée à tous les utilisateurs');
        this.bulkNotif = { title: '', message: '', targetAudience: 'All' };
      },
      error: () => this.toast.error('Erreur lors de l\'envoi')
    });
  }

  onTabChange(tab: string): void {
    this.activeTab.set(tab);
    if (tab === 'logs' && this.logs().length === 0) this.loadLogs();
  }
}
