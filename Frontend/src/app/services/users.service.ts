// users.service.ts (Reverted to original, but simplified)

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
  // This signal is updated either by the navigation state (cache hit)
  // or by the API call (cache miss)
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  getUsers(page?: number, pageSize?: number) {
    // This function must now ONLY fetch data, as the component handles the cache check.
    console.log(`Service: Performing API fetch for page ${page}`);
    return this.http
      .get<Member[]>(`${this.baseUrl}users/`, {
        observe: 'response',
        params: getPaginationParams(page, pageSize),
      })
      .subscribe({
        next: (response) => {
          this.paginatedResult.set(getPaginatedResult(response));
        },
      });
  }

  getUser(username: string) {
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }
}
