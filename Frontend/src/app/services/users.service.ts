import { HttpClient, HttpParams } from '@angular/common/http';
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
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  getUsers(page?: number, pageSize?: number) {
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
    // TODO: Implement caching for user with username
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }
}
