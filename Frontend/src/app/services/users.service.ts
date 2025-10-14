import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { PaginatedResult } from '../models/pagination';
import { Member } from '../models/member';
import { getPaginatedResult } from '../utils/pagination.utils';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  getUsers(page?: number, pageSize?: number) {
    let params = new HttpParams();

    if (page && pageSize) {
      params = params.append('page', page);
      params = params.append('pageSize', pageSize);
    }

    return this.http
      .get<Member[]>(`${this.baseUrl}users/`, {
        observe: 'response',
        params,
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
