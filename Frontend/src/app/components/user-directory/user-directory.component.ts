import { Component, inject, signal, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { LoadingService } from '../../services/loading.service';
import { Member } from '../../models/member';
import { PaginatedResult } from '../../models/pagination';
import { getPaginatedResult } from '../../utils/pagination.utils';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { QUERY_PARAMS } from '../../constants/query.constants';

@Component({
  selector: 'app-user-directory',
  templateUrl: './user-directory.component.html',
  imports: [PaginationControlsComponent],
})
export class UserDirectoryComponent {
  usersService = inject(UsersService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  users = signal<Member[]>([]);
  page = signal(1);
  pageSize = signal(20);
  currentPage = signal(1);

  private cache: Record<string, PaginatedResult<Member[]>> = {};

  constructor() {
    this.route.queryParams.subscribe((params) => {
      const pageParam = Number(params['p']);
      if (!isNaN(pageParam) && pageParam > 0 && this.page() != pageParam) {
        this.page.set(pageParam);
      }
      let pageSizeParam = Number(params['s']);
      if (!isNaN(pageSizeParam) && this.pageSize() != pageSizeParam) {
        pageSizeParam = Math.max(QUERY_PARAMS.PAGE_SIZE.MIN, pageSizeParam);
        pageSizeParam = Math.min(QUERY_PARAMS.PAGE_SIZE.MAX, pageSizeParam);
        this.pageSize.set(pageSizeParam);
      }
    });

    effect(() => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { p: this.page(), s: this.pageSize() },
      });
    });
  }

  fetchUsersEffect = effect(() => {
    const queryKey = `p=${this.page()}&s=${this.pageSize()}`;

    if (this.cache[queryKey]) {
      const current = this.users();
      current.splice(0, current.length, ...this.cache[queryKey].items!);
      this.currentPage.set(this.page());
      return;
    }

    this.loadingService.busy('user-directory');
    this.usersService.getUsers(this.page(), this.pageSize()).subscribe({
      next: (res) => {
        this.loadingService.idle('user-directory');
        const result = getPaginatedResult(res);
        this.cache[queryKey] = result;
        const current = this.users();
        current.splice(0, current.length, ...result.items!);
        this.currentPage.set(this.page());
      },
      error: () => this.loadingService.idle('user-directory'),
    });
  });

  onPageChange(newPage: number) {
    this.page.set(newPage);
  }

  onPageSizeChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const newSize = Number(input.value);
    this.pageSize.set(newSize);
    this.page.set(1);
  }

  get paginatedResult(): PaginatedResult<Member[]> | null {
    const queryKey = `p=${this.page()}&s=${this.pageSize()}`;
    return this.cache[queryKey] || null;
  }
}
