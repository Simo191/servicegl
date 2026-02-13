import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  template: `
    <div class="d-flex align-items-center justify-content-between mb-4">
      <h4 class="fw-bold mb-0">Catalogue Produits</h4>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm"><i class="bi bi-upload me-1"></i>Import CSV</button>
        <button class="btn btn-primary btn-sm"><i class="bi bi-plus-lg me-1"></i>Produit</button>
      </div>
    </div>

    <div class="table-card mb-3">
      <div class="card-header">
        <div class="input-group" style="max-width:300px">
          <span class="input-group-text"><i class="bi bi-search"></i></span>
          <input type="text" class="form-control" placeholder="Rechercher un produit..." [(ngModel)]="search" />
        </div>
        <select class="form-select" style="width:auto" [(ngModel)]="categoryFilter">
          <option value="">Tous les rayons</option>
          @for (cat of departments; track cat) { <option [value]="cat">{{ cat }}</option> }
        </select>
      </div>
    </div>

    <div class="table-card">
      <div class="table-responsive">
        <table class="table table-hover mb-0">
          <thead><tr><th>Produit</th><th>Rayon</th><th>Code-barre</th><th>Prix</th><th>Stock</th><th>Statut</th><th class="text-end">Actions</th></tr></thead>
          <tbody>
            @for (p of filteredProducts(); track p.id) {
              <tr>
                <td class="fw-medium">{{ p.name }}</td>
                <td><span class="badge bg-light text-dark">{{ p.department }}</span></td>
                <td class="font-monospace small">{{ p.barcode }}</td>
                <td class="fw-bold">{{ p.price | number:'1.2-2' }} MAD</td>
                <td [class.text-danger]="p.stock < 10" [class.fw-bold]="p.stock < 10">{{ p.stock }}</td>
                <td>
                  @if (p.stock > 0) { <span class="badge bg-success-subtle text-success">En stock</span> }
                  @else { <span class="badge bg-danger-subtle text-danger">Rupture</span> }
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
  `
})
export class CatalogManagementComponent {
  search = ''; categoryFilter = '';
  departments = ['Fruits & Légumes', 'Viandes & Poissons', 'Épicerie', 'Boissons', 'Produits laitiers', 'Hygiène & Beauté'];

  products = signal([
    { id: '1', name: 'Tomates rondes 1kg', department: 'Fruits & Légumes', barcode: '6111234567890', price: 12.90, stock: 150 },
    { id: '2', name: 'Poulet fermier', department: 'Viandes & Poissons', barcode: '6111234567891', price: 65.00, stock: 45 },
    { id: '3', name: 'Lait Centrale 1L', department: 'Produits laitiers', barcode: '6111234567892', price: 7.50, stock: 200 },
    { id: '4', name: 'Huile d\'olive 1L', department: 'Épicerie', barcode: '6111234567893', price: 55.00, stock: 8 },
    { id: '5', name: 'Eau Sidi Ali 1.5L', department: 'Boissons', barcode: '6111234567894', price: 5.50, stock: 0 },
    { id: '6', name: 'Shampoing Pantene', department: 'Hygiène & Beauté', barcode: '6111234567895', price: 42.00, stock: 35 },
  ]);

  filteredProducts() {
    let list = this.products();
    if (this.search) list = list.filter(p => p.name.toLowerCase().includes(this.search.toLowerCase()));
    if (this.categoryFilter) list = list.filter(p => p.department === this.categoryFilter);
    return list;
  }
}
