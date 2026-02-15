import { Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="text-center py-5">
      <div class="d-inline-flex align-items-center justify-content-center rounded-circle mb-3"
           style="width:72px;height:72px;background:#f1f5f9">
        <i class="bi {{ icon() }}" style="font-size:1.8rem;color:#94a3b8"></i>
      </div>
      <p class="text-muted mb-0">{{ message() }}</p>
    </div>
  `
})
export class EmptyStateComponent {
  icon = input('bi-inbox');
  message = input('Aucun résultat trouvé');
}