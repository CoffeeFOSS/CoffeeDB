import { WritableSignal } from '@angular/core';

export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  items?: T;
  pagination?: Pagination;
}

export interface PaginationSignals {
  page: { signal: WritableSignal<number>; defaultValue: number };
  pageSize: { signal: WritableSignal<number>; defaultValue: number };
}

export interface GenericSearchParams {
  page?: number;
  pageSize?: number;
}
