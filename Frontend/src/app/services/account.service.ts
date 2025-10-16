import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { map } from 'rxjs';
import { User } from '../models/user';
import { environment } from '../../environments/environment';
import { ChangePasswordPayload } from '../models/account';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null); // change any to User
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      const { role } = JSON.parse(atob(user.token.split('.')[1]));
      return Array.isArray(role) ? role : [role];
    }
    return [];
  });

  signIn(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      }),
    );
  }

  signOut() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      }),
    );
  }

  setCurrentUser(user: User) {
    this.currentUser.set(user);
    // dont set properties other than BaseUser
    const { username, token } = user;
    localStorage.setItem(
      'user',
      JSON.stringify({
        username,
        token,
      }),
    );
  }

  changePassword(model: ChangePasswordPayload) {
    console.log('TODO API, endpoint should return new token', model);
  }
}
