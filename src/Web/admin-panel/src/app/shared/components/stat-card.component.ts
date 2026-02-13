import { Component, input } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [DecimalPipe],
  template: `
    <div class="stat-card">
      <div class="d-flex align-items-start justify-content-between">
        <div>
          <p class="stat-label mb-1">{{ label() }}</p>
          <h3 class="stat-value mb-1">
            @if (isCurrency()) {
              {{ value() | number:'1.0-0' }} <small class="fs-6 fw-normal">MAD</small>
            } @else {
              {{ value() | number:'1.0-0' }}
            }
          </h3>
          @if (change() !== null) {
            <span class="stat-change" [class.text-success]="change()! >= 0" [class.text-danger]="change()! < 0">
              <i class="bi" [class.bi-arrow-up]="change()! >= 0" [class.bi-arrow-down]="change()! < 0"></i>
              {{ change()! >= 0 ? '+' : '' }}{{ change() }}%
              <span class="text-muted fw-normal"> vs mois dernier</span>
            </span>
          }
        </div>
        <div class="stat-icon" [style.background-color]="bgColor()">
          <i class="bi" [class]="icon()" [style.color]="iconColor()"></i>
        </div>
      </div>
    </div>
  `
})
export class StatCardComponent {
  label = input('');
  value = input(0);
  change = input<number | null>(null);
  icon = input('bi-bar-chart');
  bgColor = input('#eef2ff');
  iconColor = input('#4f46e5');
  isCurrency = input(false);
}
