import { Component } from '@angular/core';

@Component({
  selector: 'app-loading',
  standalone: true,
  template: `
    <div class="d-flex flex-column align-items-center justify-content-center py-5">
      <div class="spinner-border text-primary mb-3" style="width:2.5rem;height:2.5rem" role="status">
        <span class="visually-hidden">Chargement...</span>
      </div>
      <p class="text-muted mb-0">Chargement en cours...</p>
    </div>
  `
})
export class LoadingSpinnerComponent {}