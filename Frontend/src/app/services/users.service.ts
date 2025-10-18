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

          // CAUTION: There is still a pitfall if we decide to implement sorting.
          // Sorting data should be handled by the backend, since we dont want to
          // return every entry then sort on the frontend.
          // https://github.com/robchendev/CoffeeDB/issues/44

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
