import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { PaginatedResult } from '../models/pagination';
import { Member } from '../models/member';
import {
  getPaginatedResult,
  getPaginationParams,
} from '../utils/pagination.utils';
import { LoadingService } from './loading.service';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  private loadingService = inject(LoadingService);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  getUsers(page?: number, pageSize?: number, loadingId?: string) {
    if (loadingId) this.loadingService.busy(loadingId);

    return this.http
      .get<Member[]>(`${this.baseUrl}users/`, {
        observe: 'response',
        params: getPaginationParams(page, pageSize),
      })
      .subscribe({
        next: (response) => {
          if (loadingId) this.loadingService.idle(loadingId);
          this.paginatedResult.set(getPaginatedResult(response));
        },
        error: (error) => {
          if (loadingId) this.loadingService.idle(loadingId);
        },
      });
  }

  getUser(username: string) {
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }
}
