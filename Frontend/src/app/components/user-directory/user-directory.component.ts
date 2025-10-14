import { Component, effect, inject, signal } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-user-directory',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationControlsComponent],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss',
})
export class UserDirectoryComponent {
  usersService = inject(UsersService);
  page = signal(1);
  pageSize = signal(20);

  fetchUsersEffect = effect(() => {
    this.usersService.getUsers(this.page(), this.pageSize());
  });
}
