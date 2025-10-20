import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Member } from '../models/member';
import { getPaginationParams } from '../utils/pagination.utils';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentPageSize: number | null = null;

  getUsers(page?: number, pageSize?: number) {
    return this.http.get<Member[]>(`${this.baseUrl}users/`, {
      observe: 'response',
      params: getPaginationParams(page, pageSize),
    });
  }

  getUser(username: string) {
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }
}
