import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="position-fixed bottom-0 end-0 p-3" style="z-index:9999">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast show mb-2 border-0 shadow"
             [class.bg-success]="toast.type === 'success'"
             [class.bg-danger]="toast.type === 'error'"
             [class.bg-warning]="toast.type === 'warning'"
             [class.bg-info]="toast.type === 'info'"
             [class.text-white]="toast.type !== 'warning'">
          <div class="toast-body d-flex align-items-center justify-content-between">
            <span>{{ toast.message }}</span>
            <button class="btn-close btn-close-white ms-2" (click)="toastService.remove(toast.id)"></button>
          </div>
        </div>
      }
    </div>
  `
})
export class ToastContainerComponent {
  toastService = inject(ToastService);
}
