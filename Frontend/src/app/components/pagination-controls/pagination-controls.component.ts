import { Component, input, output, Signal } from '@angular/core';
import { PaginatedResult } from '../../models/pagination';

@Component({
  selector: 'app-pagination-controls',
  imports: [],
  templateUrl: './pagination-controls.component.html',
  styleUrls: ['./pagination-controls.component.scss'],
})
export class PaginationControlsComponent<T> {
  minPageSize = input<number>(5);
  maxPageSize = input<number>(50);

  page = input.required<number>();
  pageChange = output<number>(); // for [(page)}

  pageSize = input.required<number>();
  pageSizeChange = output<number>(); // for [(pageSize)}

  paginatedResult = input.required<PaginatedResult<T[]> | null>();
  pageSizeInput = 5;

  allowPageSizeEdit = input<boolean>(false);

  onPageChange(newPage: number) {
    if (
      newPage < 1 ||
      newPage > this.paginatedResult()?.pagination?.totalPages!
    ) {
      return;
    }
    this.pageChange.emit(newPage);
  }

  onPageSizeInput($event: Event) {
    this.pageSizeInput = Number(($event.target as HTMLInputElement).value);
  }

  onPageSizeChange() {
    if (!this.allowPageSizeEdit) return;

    const normalizedSize = Math.max(
      this.minPageSize(),
      Math.min(this.pageSizeInput, this.maxPageSize()),
    );

    this.pageSizeChange.emit(normalizedSize);
    this.pageChange.emit(1);
    this.pageSizeInput = normalizedSize;
  }
}
