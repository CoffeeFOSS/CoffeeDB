import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getUsers() {
    return this.http.get<User[]>(`${this.baseUrl}users/`);
  }

  getUser(id: string) {
    return this.http.get<User>(`${this.baseUrl}users/${id}`);
  }
}
