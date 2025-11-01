import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { UserWithRoles } from '../models/user';
import { getHttpParams } from '../utils/params.utils';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUserWithRoles(page?: number, pageSize?: number) {
    return this.http.get<UserWithRoles[]>(
      this.baseUrl + 'admin/users-with-roles',
      {
        observe: 'response',
        params: getHttpParams({
          page,
          pageSize,
        }),
      },
    );
  }

  editUserRoles(username: string, roles: string[]) {
    const rolesStr = roles.length ? roles.join(',') : 'Empty';
    return this.http.post<string[]>(
      this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + rolesStr,
      {},
    );
  }
}
