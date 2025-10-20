import { Component, effect, inject, signal } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-user-directory',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationControlsComponent],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss',
})
export class UserDirectoryComponent {
  usersService = inject(UsersService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);
  page = signal(1);
  pageSize = signal(20);

  constructor() {
    this.route.queryParams.subscribe((params) => {
      const pageParam = Number(params['page']);
      if (!isNaN(pageParam) && pageParam > 0 && this.page() != pageParam) {
        this.page.set(pageParam);
      }
    });

    effect(() => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { page: this.page() },
      });
    });
  }

  fetchUsersEffect = effect(() => {
    if (this.usersService.paginatedResultMap()[this.page()]) return;
    this.usersService.getUsers(this.page(), this.pageSize(), 'user-directory');
  });
}
