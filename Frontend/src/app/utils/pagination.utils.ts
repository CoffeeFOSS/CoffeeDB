import { HttpParams, HttpResponse } from '@angular/common/http';

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
