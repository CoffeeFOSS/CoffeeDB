import { Component, input, output } from '@angular/core';
import { PaginatedResult } from '../../models/pagination';

@Component({
  selector: 'app-pagination-controls',
  imports: [],
  templateUrl: './pagination-controls.component.html',
  styleUrls: ['./pagination-controls.component.scss'],
})
export class PaginationControlsComponent<T> {
  page = input.required<number>();
  pageSize = input.required<number>(); // input signal, read-only
  paginatedResult = input.required<PaginatedResult<T[]> | null>();
  onLoad = output();
  onPageChange_ = output<number>();
  onPageSizeChange_ = output<number>(); // emit new page size
  pageSizeInput = 5;

  // TODO: This is bloated, optimize

  onPageChange(newPage: number) {
    if (
      newPage < 1 ||
      newPage > this.paginatedResult()?.pagination?.totalPages!
    ) {
      return;
    }
    this.onPageChange_.emit(newPage);
    this.onLoad.emit();
  }

  onPageSizeInput($event: Event) {
    const inputEl = $event.target as HTMLInputElement;
    this.pageSizeInput = Number(inputEl.value);
  }

  onPageSizeChange() {
    const normalizedSize = Math.max(5, Math.min(this.pageSizeInput, 50));
    this.onPageSizeChange_.emit(normalizedSize);

    this.onPageChange_.emit(1);
    this.onLoad.emit();

    this.pageSizeInput = normalizedSize;
  }
}
