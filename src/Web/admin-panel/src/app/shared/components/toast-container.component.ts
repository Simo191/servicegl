import { Component } from '@angular/core';
import { ToastService, Toast } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index:9999">
      @for (t of toasts(); track t.id) {
        <div class="toast show border-0 shadow-sm mb-2" role="alert" style="border-radius:10px;min-width:300px"
             [style.border-left]="'4px solid ' + getColor(t.type)">
          <div class="toast-body d-flex align-items-center gap-2 py-3 px-3">
            <i class="bi" [class]="getIcon(t.type)" [style.color]="getColor(t.type)"></i>
            <span class="flex-grow-1" style="font-size:0.88rem">{{ t.message }}</span>
            <button type="button" class="btn-close btn-close-sm" (click)="dismiss(t.id)"></button>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .btn-close-sm { font-size: 0.6rem; }
  `]
})
export class ToastContainerComponent {
  constructor(private toastService: ToastService) {}
  toasts = () => this.toastService.toasts();
  dismiss(id: number) { this.toastService.dismiss(id); }
  getIcon(type: string): string {
    return { success: 'bi-check-circle-fill', error: 'bi-x-circle-fill', warning: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' }[type] || 'bi-info-circle-fill';
  }
  getColor(type: string): string {
    return { success: '#10b981', error: '#ef4444', warning: '#f59e0b', info: '#6366f1' }[type] || '#6366f1';
  }
}