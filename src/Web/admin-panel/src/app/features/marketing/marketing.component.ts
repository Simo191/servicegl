import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { AdminService } from '../../core/services/admin.service';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { ConfirmModalComponent } from '../../shared/components/confirm-modal.component';
import { PromoCodeDto, CreatePromotionRequest } from '../../core/models/api.models';

@Component({
  selector: 'app-marketing',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe, ConfirmModalComponent],
  templateUrl: './marketing.component.html',
  styleUrls: ['./marketing.component.scss']
})
export class MarketingComponent implements OnInit {
  promos = signal<PromoCodeDto[]>([]);
  showCreateForm = signal(false);

  showConfirm = signal(false);
  confirmTitle = signal('');
  confirmMessage = signal('');
  confirmText = signal('Confirmer');
  private pendingAction: (() => void) | null = null;

  newPromo: CreatePromotionRequest = {
    code: '', title: '', discountType: 'Percentage', discountValue: 0,
    minOrderAmount: 0, maxDiscount: 0, startDate: '', endDate: '',
    maxUsages: 100, applicableModule: 'All', freeDelivery: false
  };

  constructor(private admin: AdminService, private api: ApiService, private toast: ToastService) {}
  ngOnInit(): void { this.loadPromos(); }

  loadPromos(): void {
    this.api.get<PromoCodeDto[]>('admin/promo-codes').subscribe({
      next: res => { if (res.success) this.promos.set(res.data); },
      error: () => {
        this.promos.set([
          { id: '1', code: 'WELCOME20', title: 'Bienvenue', discountType: 'Percentage', discountValue: 20, minOrderAmount: 50, maxDiscount: 100, maxUsages: 1000, usedCount: 342, startDate: '2025-01-01', endDate: '2025-12-31', isActive: true, applicableModule: 'All', freeDelivery: false },
          { id: '2', code: 'LIVGRATUITE', title: 'Livraison offerte', discountType: 'FreeDelivery', discountValue: 0, minOrderAmount: 100, maxDiscount: 0, maxUsages: 500, usedCount: 189, startDate: '2025-06-01', endDate: '2025-06-30', isActive: true, applicableModule: 'All', freeDelivery: true },
          { id: '3', code: 'ETE50', title: 'Promo été', discountType: 'Fixed', discountValue: 50, minOrderAmount: 200, maxDiscount: 50, maxUsages: 200, usedCount: 200, startDate: '2025-05-01', endDate: '2025-05-31', isActive: false, applicableModule: 'Restaurant', freeDelivery: false },
        ]);
      }
    });
  }

  createPromo(): void {
    if (!this.newPromo.code || !this.newPromo.title) {
      this.toast.warning('Veuillez remplir le code et le titre');
      return;
    }
    this.admin.createPromotion(this.newPromo).subscribe({
      next: () => {
        this.toast.success('Code promo créé avec succès !');
        this.showCreateForm.set(false);
        this.resetForm();
        this.loadPromos();
      },
      error: () => this.toast.error('Erreur lors de la création du code promo')
    });
  }

  togglePromo(p: PromoCodeDto): void {
    const action = p.isActive ? 'Désactiver' : 'Réactiver';
    this.confirmTitle.set(action + ' le code promo');
    this.confirmMessage.set(action + ' le code "' + p.code + '" ?');
    this.confirmText.set(action);
    this.pendingAction = () => {
      this.api.patch('admin/promo-codes/' + p.id + '/toggle', {}).subscribe({
        next: () => { this.toast.success('Statut mis à jour'); this.loadPromos(); },
        error: () => this.toast.error('Erreur')
      });
    };
    this.showConfirm.set(true);
  }

  onConfirm(): void { this.showConfirm.set(false); this.pendingAction?.(); }
  onCancel(): void { this.showConfirm.set(false); this.pendingAction = null; }

  private resetForm(): void {
    this.newPromo = { code: '', title: '', discountType: 'Percentage', discountValue: 0, minOrderAmount: 0, maxDiscount: 0, startDate: '', endDate: '', maxUsages: 100, applicableModule: 'All', freeDelivery: false };
  }
}
