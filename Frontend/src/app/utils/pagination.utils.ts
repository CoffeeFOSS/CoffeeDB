import { HttpParams, HttpResponse } from '@angular/common/http';
import { PaginatedResult } from '../models/pagination';

export function getPaginatedResult<T>(response: HttpResponse<T[]>) {
  return {
    items: response.body as T[],
    pagination: JSON.parse(response.headers.get('Pagination')!),
  };
}

export function getPaginationParams(page?: number, pageSize?: number) {
  let params = new HttpParams();

  if (page && pageSize) {
    params = params.append('page', page);
    params = params.append('pageSize', pageSize);
  }

  return params;
}

export function getPaginationText(
  paginatedResult: PaginatedResult<any[]> | null,
) {
  const pagination = paginatedResult?.pagination;
  if (!pagination) return '';

  const { itemsPerPage, currentPage, totalItems } = pagination;
  const start = itemsPerPage * (currentPage - 1) + 1;
  const end = Math.min(itemsPerPage * currentPage, totalItems);

  return `${start}-${end} of ${pagination.totalItems}`;
}
