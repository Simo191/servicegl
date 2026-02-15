import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  template: `
    @if (show()) {
      <div class="modal-backdrop fade show" (click)="cancelled.emit()"></div>
      <div class="modal fade show d-block" tabindex="-1" (click)="cancelled.emit()">
        <div class="modal-dialog modal-dialog-centered" (click)="$event.stopPropagation()">
          <div class="modal-content border-0 shadow-lg" style="border-radius:14px">
            <div class="modal-header border-0 pb-0">
              <h6 class="modal-title fw-bold">{{ title() }}</h6>
              <button type="button" class="btn-close" (click)="cancelled.emit()"></button>
            </div>
            <div class="modal-body">
              <p class="text-muted mb-0">{{ message() }}</p>
            </div>
            <div class="modal-footer border-0 pt-0">
              <button class="btn btn-light" (click)="cancelled.emit()">Annuler</button>
              <button class="btn btn-danger" (click)="confirmed.emit()">{{ confirmText() }}</button>
            </div>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .modal-backdrop { z-index: 1050; }
    .modal { z-index: 1055; }
  `]
})
export class ConfirmModalComponent {
  show = input(false);
  title = input('Confirmation');
  message = input('Êtes-vous sûr ?');
  confirmText = input('Confirmer');
  confirmed = output<void>();
  cancelled = output<void>();
}