import { Component, inject, signal, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { LoadingService } from '../../services/loading.service';
import { Member } from '../../models/member';
import { PaginatedResult } from '../../models/pagination';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { QUERY_PARAMS } from '../../constants/query.constants';
import {
  createGetPaginatedResult,
  createOnPageChange,
  createOnPageSizeChange,
  extractAndSetParams,
  fetchItemsWithCache,
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
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Member[]>> = {};

  private signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
  };

  constructor() {
    this.route.queryParams.subscribe((params) =>
      extractAndSetParams(params, this.signalDefaults),
    );
    effect(() =>
      syncParamsWithUrl({
        router: this.router,
        route: this.route,
        signalDefaults: this.signalDefaults,
      }),
    );
    effect(() => {
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
  }

  onPageChange = createOnPageChange(this.page);
  onPageSizeChange = createOnPageSizeChange(this.page, this.pageSize);

  get paginatedResult(): PaginatedResult<Member[]> | null {
    return createGetPaginatedResult<Member>(this.cache, this.signalDefaults);
  }
}
