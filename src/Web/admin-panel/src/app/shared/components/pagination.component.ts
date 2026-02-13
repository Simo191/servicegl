import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  template: `
    @if (totalPages() > 1) {
      <nav>
        <ul class="pagination pagination-sm justify-content-center mb-0">
          <li class="page-item" [class.disabled]="currentPage() === 1">
            <a class="page-link" (click)="pageChange.emit(currentPage() - 1)" style="cursor:pointer">
              <i class="bi bi-chevron-left"></i>
            </a>
          </li>
          @for (page of pages(); track page) {
            <li class="page-item" [class.active]="page === currentPage()">
              <a class="page-link" (click)="pageChange.emit(page)" style="cursor:pointer">{{ page }}</a>
            </li>
          }
          <li class="page-item" [class.disabled]="currentPage() === totalPages()">
            <a class="page-link" (click)="pageChange.emit(currentPage() + 1)" style="cursor:pointer">
              <i class="bi bi-chevron-right"></i>
            </a>
          </li>
        </ul>
      </nav>
    }
  `
})
export class PaginationComponent {
  currentPage = input(1);
  totalPages = input(1);
  pageChange = output<number>();

  pages(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }
}
