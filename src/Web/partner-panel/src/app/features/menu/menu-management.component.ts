import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-menu-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Gestion du Menu</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Catégorie</button>
        <button class="btn btn-primary btn-sm" (click)="showAddItem.set(true)"><i class="bi bi-plus-lg me-1"></i>Plat</button>
      </div>
    </div>

    @if (showAddItem()) {
      <div class="table-card mb-4">
        <div class="card-header"><h6 class="fw-bold mb-0">Ajouter un plat</h6></div>
        <div class="card-body p-3">
          <div class="row g-3">
            <div class="col-md-4"><label class="form-label small">Nom</label><input class="form-control" [(ngModel)]="newItem.name" /></div>
            <div class="col-md-3"><label class="form-label small">Catégorie</label>
              <select class="form-select" [(ngModel)]="newItem.category">
                @for (cat of categories; track cat) { <option [value]="cat">{{ cat }}</option> }
              </select>
            </div>
            <div class="col-md-2"><label class="form-label small">Prix (MAD)</label><input type="number" class="form-control" [(ngModel)]="newItem.price" /></div>
            <div class="col-md-3"><label class="form-label small">Description</label><input class="form-control" [(ngModel)]="newItem.description" /></div>
            <div class="col-12">
              <button class="btn btn-primary btn-sm me-2" (click)="addItem()">Ajouter</button>
              <button class="btn btn-light btn-sm" (click)="showAddItem.set(false)">Annuler</button>
            </div>
          </div>
        </div>
      </div>
    }

    @for (cat of categories; track cat) {
      <div class="table-card mb-3">
        <div class="card-header">
          <h6 class="fw-bold mb-0">{{ cat }}</h6>
          <span class="badge bg-light text-dark">{{ itemsByCategory(cat).length }} plats</span>
        </div>
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead><tr><th>Plat</th><th>Description</th><th>Prix</th><th>Disponible</th><th class="text-end">Actions</th></tr></thead>
            <tbody>
              @for (item of itemsByCategory(cat); track item.id) {
                <tr>
                  <td class="fw-medium">{{ item.name }}</td>
                  <td class="text-muted small">{{ item.description }}</td>
                  <td class="fw-bold">{{ item.price | number:'1.2-2' }} MAD</td>
                  <td>
                    <div class="form-check form-switch">
                      <input class="form-check-input" type="checkbox" [checked]="item.available" (change)="toggleAvailability(item)">
                    </div>
                  </td>
                  <td class="text-end">
                    <button class="btn btn-sm btn-outline-primary me-1"><i class="bi bi-pencil"></i></button>
                    <button class="btn btn-sm btn-outline-danger"><i class="bi bi-trash"></i></button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    }
  `
})
export class MenuManagementComponent {
  showAddItem = signal(false);
  categories = ['Entrées', 'Plats principaux', 'Desserts', 'Boissons'];
  newItem = { name: '', category: 'Entrées', price: 0, description: '' };

  items = signal([
    { id: '1', name: 'Harira', category: 'Entrées', description: 'Soupe traditionnelle', price: 25, available: true },
    { id: '2', name: 'Briouates', category: 'Entrées', description: 'Briouates au fromage', price: 35, available: true },
    { id: '3', name: 'Tajine Poulet', category: 'Plats principaux', description: 'Poulet aux olives et citron', price: 85, available: true },
    { id: '4', name: 'Couscous Royal', category: 'Plats principaux', description: '7 légumes, agneau', price: 120, available: true },
    { id: '5', name: 'Pastilla', category: 'Plats principaux', description: 'Au pigeon', price: 95, available: false },
    { id: '6', name: 'Corne de gazelle', category: 'Desserts', description: 'Pâtisserie amande', price: 30, available: true },
    { id: '7', name: 'Thé à la menthe', category: 'Boissons', description: 'Thé vert menthe', price: 15, available: true },
    { id: '8', name: 'Jus d\'orange frais', category: 'Boissons', description: 'Pressé maison', price: 20, available: true },
  ]);

  constructor(private toast: ToastService) {}
  itemsByCategory(cat: string) { return this.items().filter(i => i.category === cat); }
  toggleAvailability(item: any) { item.available = !item.available; this.toast.info(item.available ? 'Plat activé' : 'Plat désactivé'); }
  addItem(): void { this.items.update(list => [...list, { ...this.newItem, id: Date.now().toString(), available: true }]); this.showAddItem.set(false); this.toast.success('Plat ajouté'); }
}
