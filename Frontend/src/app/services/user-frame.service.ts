import { Injectable, signal } from '@angular/core';
import { Member } from '../models/member';

@Injectable({
  providedIn: 'root',
})
export class UserFrameService {
  username = signal<string | null>(null);
  user = signal<Member | null>(null);

  reset() {
    this.username.set(null);
    this.user.set(null);
  }
}
