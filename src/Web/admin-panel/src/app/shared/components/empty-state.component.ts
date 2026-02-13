import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="text-center p-5">
      <i class="bi" [class]="icon()" style="font-size:3rem;color:#cbd5e1"></i>
      <p class="text-muted mt-2 mb-0">{{ message() }}</p>
    </div>
  `
})
export class EmptyStateComponent {
  icon = input('bi-inbox');
  message = input('Aucune donnée trouvée');
}
