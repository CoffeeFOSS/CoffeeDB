import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { PaginatedResult } from '../models/pagination';
import { Member } from '../models/member';
import {
  getPaginatedResult,
  getPaginationParams,
} from '../utils/pagination.utils';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentPageSize: number | null = null;
  paginatedResultMap = signal<Record<number, PaginatedResult<Member[]>>>({});

  getUsers(page?: number, pageSize?: number) {
    return this.http
      .get<Member[]>(`${this.baseUrl}users/`, {
        observe: 'response',
        params: getPaginationParams(page, pageSize),
      })
      .subscribe({
        next: (response) => {
          const result = getPaginatedResult(response);

          // if user modified page size, reset paginatedResultMap
          if (
            this.currentPageSize &&
            this.currentPageSize != result.pagination.itemsPerPage
          ) {
            this.paginatedResultMap.set({});
          }
          this.currentPageSize = result.pagination.itemsPerPage;

          // update paginatedResultMap
          this.paginatedResultMap.set({
            ...this.paginatedResultMap(),
            [result.pagination.currentPage]: result,
          });
        },
      });
  }

  getUser(username: string) {
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }
}
