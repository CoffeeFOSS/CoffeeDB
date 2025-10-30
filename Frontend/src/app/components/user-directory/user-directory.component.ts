import { Component, inject, signal, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { LoadingService } from '../../services/loading.service';
import { Member } from '../../models/member';
import { PaginatedResult } from '../../models/pagination';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { QUERY_PARAMS } from '../../constants/query.constants';
import {
  extractAndSetParams,
  fetchItemsWithCache,
  getQueryKey,
  syncParamsWithUrl,
} from '../../utils/params.utils';

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
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);

  signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
  };

  private cache: Record<string, PaginatedResult<Member[]>> = {};

  constructor() {
    const { router, route, signalDefaults } = this;
    this.route.queryParams.subscribe((params) =>
      extractAndSetParams(params, this.signalDefaults),
    );
    effect(() => syncParamsWithUrl({ router, route, signalDefaults }));
  }

  fetchUsersEffect = effect(() => {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.users,
      currentPageSignal: this.currentPage,
      signalDefaults: this.signalDefaults,
      loadingKey: 'user-directory',
      loadingService: this.loadingService,
      fetchPaginatedItems: () =>
        this.usersService.getUsers(this.page(), this.pageSize()),
    });
  });

  onPageChange(newPage: number) {
    this.page.set(newPage);
  }

  onPageSizeChange(event: Event) {
    const input = event.target as HTMLInputElement;
    const newSize = Number(input.value);
    this.pageSize.set(newSize);
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
  }

  get paginatedResult(): PaginatedResult<Member[]> | null {
    const queryKey = getQueryKey(this.signalDefaults);
    return this.cache[queryKey] || null;
  }
}
