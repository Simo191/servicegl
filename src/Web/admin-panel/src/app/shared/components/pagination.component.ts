import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  template: `
    @if (totalPages() > 1) {
      <nav>
        <ul class="pagination pagination-sm mb-0 gap-1">
          <li class="page-item" [class.disabled]="currentPage() <= 1">
            <button class="page-link" (click)="pageChange.emit(currentPage() - 1)">
              <i class="bi bi-chevron-left"></i>
            </button>
          </li>
          @for (p of pages(); track p) {
            @if (p === -1) {
              <li class="page-item disabled"><span class="page-link">…</span></li>
            } @else {
              <li class="page-item" [class.active]="p === currentPage()">
                <button class="page-link" (click)="pageChange.emit(p)">{{ p }}</button>
              </li>
            }
          }
          <li class="page-item" [class.disabled]="currentPage() >= totalPages()">
            <button class="page-link" (click)="pageChange.emit(currentPage() + 1)">
              <i class="bi bi-chevron-right"></i>
            </button>
          </li>
        </ul>
      </nav>
    }
  `,
  styles: [`
    .page-link { border-radius: 6px !important; min-width: 34px; text-align: center; font-size: 0.82rem; }
    .page-item.active .page-link { background: #6366f1; border-color: #6366f1; }
  `]
})
export class PaginationComponent {
  currentPage = input(1);
  totalPages = input(1);
  pageChange = output<number>();

  pages = () => {
    const t = this.totalPages();
    const c = this.currentPage();
    if (t <= 7) return Array.from({ length: t }, (_, i) => i + 1);
    const pages: number[] = [1];
    if (c > 3) pages.push(-1);
    for (let i = Math.max(2, c - 1); i <= Math.min(t - 1, c + 1); i++) pages.push(i);
    if (c < t - 2) pages.push(-1);
    pages.push(t);
    return pages;
  };
}