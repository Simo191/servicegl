import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  template: `
    @if (show()) {
      <div class="modal d-block" tabindex="-1" style="background:rgba(0,0,0,0.5)">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content border-0 shadow">
            <div class="modal-header border-0">
              <h5 class="modal-title">{{ title() }}</h5>
              <button class="btn-close" (click)="cancelled.emit()"></button>
            </div>
            <div class="modal-body">
              <p class="text-muted mb-0">{{ message() }}</p>
            </div>
            <div class="modal-footer border-0">
              <button class="btn btn-light" (click)="cancelled.emit()">Annuler</button>
              <button class="btn" [class]="'btn-' + confirmColor()" (click)="confirmed.emit()">
                {{ confirmText() }}
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class ConfirmModalComponent {
  show = input(false);
  title = input('Confirmation');
  message = input('Êtes-vous sûr ?');
  confirmText = input('Confirmer');
  confirmColor = input('danger');
  confirmed = output<void>();
  cancelled = output<void>();
}
