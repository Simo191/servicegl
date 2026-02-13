import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-partner-orders',
  standalone: true,
  imports: [FormsModule, DecimalPipe, DatePipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Mes Commandes</h4>
      <div class="btn-group">
        @for (tab of tabs; track tab.value) {
          <button class="btn btn-sm" [class.btn-primary]="selectedTab === tab.value" [class.btn-outline-primary]="selectedTab !== tab.value" (click)="selectedTab = tab.value">
            {{ tab.label }} ({{ tab.count }})
          </button>
        }
      </div>
    </div>

    <div class="row g-3">
      @for (order of filteredOrders(); track order.id) {
        <div class="col-12">
          <div class="table-card">
            <div class="card-body p-3">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <h6 class="fw-bold mb-1">#{{ order.number }} - {{ order.customer }}</h6>
                  <p class="text-muted small mb-1">{{ order.items }}</p>
                  <span class="badge" [class]="order.statusClass">{{ order.statusLabel }}</span>
                </div>
                <div class="text-end">
                  <h5 class="fw-bold text-primary mb-1">{{ order.amount | number:'1.2-2' }} MAD</h5>
                  <small class="text-muted">{{ order.time }}</small>
                </div>
              </div>
              @if (order.status === 'pending') {
                <div class="mt-3 d-flex gap-2">
                  <button class="btn btn-success btn-sm" (click)="acceptOrder(order)"><i class="bi bi-check-lg me-1"></i>Accepter</button>
                  <button class="btn btn-outline-danger btn-sm" (click)="rejectOrder(order)"><i class="bi bi-x-lg me-1"></i>Refuser</button>
                </div>
              }
              @if (order.status === 'preparing') {
                <div class="mt-3">
                  <button class="btn btn-primary btn-sm" (click)="markReady(order)"><i class="bi bi-check-circle me-1"></i>Marquer prête</button>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class PartnerOrdersComponent {
  selectedTab = 'all';
  tabs = [
    { value: 'all', label: 'Toutes', count: 12 },
    { value: 'pending', label: 'Nouvelles', count: 3 },
    { value: 'preparing', label: 'En cours', count: 4 },
    { value: 'ready', label: 'Prêtes', count: 2 },
    { value: 'delivered', label: 'Livrées', count: 3 },
  ];

  orders = signal([
    { id: '1', number: 'CMD-456', customer: 'Ahmed B.', items: '2x Tajine, 1x Couscous, 2x Thé', amount: 285, status: 'pending', statusLabel: 'Nouvelle', statusClass: 'bg-warning', time: 'Il y a 2 min' },
    { id: '2', number: 'CMD-455', customer: 'Sara L.', items: '1x Pizza Margherita, 1x Tiramisu', amount: 135, status: 'pending', statusLabel: 'Nouvelle', statusClass: 'bg-warning', time: 'Il y a 5 min' },
    { id: '3', number: 'CMD-454', customer: 'Omar K.', items: '3x Burger, 3x Frites, 3x Coca', amount: 198, status: 'preparing', statusLabel: 'En préparation', statusClass: 'bg-primary', time: 'Il y a 15 min' },
    { id: '4', number: 'CMD-453', customer: 'Youssef M.', items: '1x Pastilla, 2x Brochettes', amount: 210, status: 'preparing', statusLabel: 'En préparation', statusClass: 'bg-primary', time: 'Il y a 22 min' },
    { id: '5', number: 'CMD-452', customer: 'Fatima Z.', items: '2x Sushi Roll, 1x Miso Soup', amount: 320, status: 'ready', statusLabel: 'Prête', statusClass: 'bg-success', time: 'Il y a 30 min' },
    { id: '6', number: 'CMD-451', customer: 'Karim T.', items: '1x Tajine Poulet', amount: 95, status: 'delivered', statusLabel: 'Livrée', statusClass: 'bg-secondary', time: 'Il y a 1h' },
  ]);

  constructor(private toast: ToastService) {}

  filteredOrders() {
    if (this.selectedTab === 'all') return this.orders();
    return this.orders().filter(o => o.status === this.selectedTab);
  }

  acceptOrder(o: any): void { o.status = 'preparing'; o.statusLabel = 'En préparation'; o.statusClass = 'bg-primary'; this.toast.success('Commande acceptée'); }
  rejectOrder(o: any): void { this.orders.update(list => list.filter(x => x.id !== o.id)); this.toast.warning('Commande refusée'); }
  markReady(o: any): void { o.status = 'ready'; o.statusLabel = 'Prête'; o.statusClass = 'bg-success'; this.toast.success('Commande marquée prête'); }
}
