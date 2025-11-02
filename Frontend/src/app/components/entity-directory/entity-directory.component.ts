import {
  Component,
  effect,
  inject,
  input,
  signal,
  untracked,
} from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { PaginatedResult } from '../../models/pagination';
import {
  extractAndSetParams,
  fetchItemsWithCache,
  resetSearchToSignalDefaults,
  SearchableSignalDefault,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { getPaginationText } from '../../utils/pagination.utils';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { QUERY_PARAMS } from '../../constants/query.constants';
import { Observable } from 'rxjs';
import { HttpResponse } from '@angular/common/http';

interface ColumnNoLink {
  header: string;
  field: string;
  link?: undefined;
}

interface ColumnExternalLink {
  header: string;
  field: string;
  link: {
    key: string;
    type: 'external';
  };
}

interface ColumnInternalIdLink {
  header: string;
  field: string;
  link: {
    key: string;
    type: 'internalId';
    rootPath: string;
  };
}

interface ColumnInternalPathLink {
  header: string;
  field: string;
  link: {
    key: string;
    type: 'internalPath';
  };
}

export type Column =
  | ColumnNoLink
  | ColumnExternalLink
  | ColumnInternalIdLink
  | ColumnInternalPathLink;

@Component({
  selector: 'app-entity-directory',
  imports: [PaginationControlsComponent, RouterLink],
  templateUrl: './entity-directory.component.html',
  styleUrl: './entity-directory.component.scss',
})
export class EntityDirectoryComponent<T> {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  columns = input.required<Column[]>();
  signalDefaults =
    input.required<Record<string, SearchableSignalDefault<any>>>();
  fetchItems =
    input.required<
      (params: Record<string, any>) => Observable<HttpResponse<T[]>>
    >();
  loadingKey = input.required<string>();

  items = signal<T[]>([]);
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  paginatedResultSignal = signal<PaginatedResult<T[]> | null>(null);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<T[]>> = {};
  private paginationSignalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: {
      signal: this.pageSize,
      defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT,
    },
  };

  ngOnInit() {
    this.route.queryParams.subscribe((params) =>
      extractAndSetParams(params, {
        ...this.paginationSignalDefaults,
        ...this.signalDefaults(),
      }),
    );
  }

  constructor() {
    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        syncParamsWithUrl({
          router: this.router,
          route: this.route,
          signalDefaults: {
            ...this.paginationSignalDefaults,
            ...this.signalDefaults(),
          },
        });
      });
    });

    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => this.fetchItemsTrigger());
    });
  }

  fetchItemsTrigger() {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.items,
      currentPageSignal: this.currentPage,
      signalDefaults: {
        ...this.paginationSignalDefaults,
        ...this.signalDefaults(),
      },
      loadingKey: this.loadingKey(),
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: () => {
        const params: Record<string, any> = {
          p: this.page(),
          s: this.pageSize(),
          ...Object.fromEntries(
            Object.entries(this.signalDefaults()).map(([key, { signal }]) => [
              key,
              signal(),
            ]),
          ),
        };
        return this.fetchItems()(params);
      },
    });
  }

  onSearch() {
    if (!this.hasSearchValue()) return;
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.onRefreshData();
  }

  onResetSearch() {
    resetSearchToSignalDefaults(this.signalDefaults());
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.onRefreshData();
  }

  onRefreshData() {
    this.fetchItemsTrigger();
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      signalDefaults: this.signalDefaults(),
    });
  }

  get paginationText(): string {
    return getPaginationText(this.paginatedResultSignal());
  }

  hasSearchValue() {
    return Object.entries(this.signalDefaults()).some(
      ([, { signal }]) => !!signal(),
    );
  }

  objectKeys = Object.keys;

  getCellValue(item: any, column: Column) {
    return item[column.field];
  }

  getLinkValue(item: any, key: string) {
    return item[key];
  }
}
