import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../models/user';
import { getPaginationParams } from '../utils/pagination.utils';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUserWithRoles(page?: number, pageSize?: number) {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles', {
      observe: 'response',
      params: getPaginationParams(page, pageSize),
    });
  }
}
