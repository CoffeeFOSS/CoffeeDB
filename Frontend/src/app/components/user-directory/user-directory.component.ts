import { Component, inject, signal, effect, untracked } from '@angular/core';
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
  resetSearchToSignalDefaults,
  syncParamsWithUrl,
} from '../../utils/params.utils';

@Component({
  selector: 'app-user-directory',
  templateUrl: './user-directory.component.html',
  imports: [PaginationControlsComponent],
})
export class UserDirectoryComponent {
  private usersService = inject(UsersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  users = signal<Member[]>([]);
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  username = signal('');
  paginatedResultSignal = signal<PaginatedResult<Member[]> | null>(null);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Member[]>> = {};

  private signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
    u: { signal: this.username, defaultValue: undefined },
  };

  constructor() {
    this.route.queryParams.subscribe((params) =>
      extractAndSetParams(params, this.signalDefaults),
    );
    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        syncParamsWithUrl({
          router: this.router,
          route: this.route,
          signalDefaults: this.signalDefaults,
        });
      });
    });

    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        this.fetchItemsTrigger();
      });
    });
  }

  fetchItemsTrigger() {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.users,
      currentPageSignal: this.currentPage,
      signalDefaults: this.signalDefaults,
      loadingKey: 'user-directory',
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: () =>
        this.usersService.getUsers({
          page: this.page(),
          pageSize: this.pageSize(),
          username: this.username(),
        }),
    });
  }

  onChangeUsername(event: Event) {
    this.username.set((event.target as HTMLInputElement).value);
  }

  onSearchUser() {
    if (!this.username()) return;
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.fetchItemsTrigger();
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      signalDefaults: this.signalDefaults,
    });
  }

  onResetSearch() {
    resetSearchToSignalDefaults(this.signalDefaults);
    this.fetchItemsTrigger();
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      signalDefaults: this.signalDefaults,
    });
  }

  //refactor
  get paginationText(): string {
    const pagination = this.paginatedResultSignal()?.pagination;
    if (!pagination) return '';

    const { itemsPerPage, currentPage, totalItems } = pagination;
    const start = itemsPerPage * (currentPage - 1) + 1;
    const end = Math.min(itemsPerPage * currentPage, totalItems);

    return `${start}-${end} of ${pagination.totalItems}`;
  }
}
