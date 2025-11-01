import { Component, effect, inject, signal, untracked } from '@angular/core';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { QUERY_PARAMS } from '../../constants/query.constants';
import { PaginatedResult } from '../../models/pagination';
import { LoadingService } from '../../services/loading.service';
import {
  extractAndSetParams,
  fetchItemsWithCache,
  resetSearchToSignalDefaults,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import { Roaster } from '../../models/roaster';
import { getPaginationText } from '../../utils/pagination.utils';

@Component({
  selector: 'app-roasters',
  imports: [PaginationControlsComponent, RouterLink],
  templateUrl: './roaster-directory.component.html',
  styleUrl: './roaster-directory.component.scss',
})
export class RoasterDirectoryComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  roasters = signal<Roaster[]>([]);
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  name = signal('');
  location = signal('');
  paginatedResultSignal = signal<PaginatedResult<Roaster[]> | null>(null);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Roaster[]>> = {};

  private signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
    n: { signal: this.name, defaultValue: undefined },
    l: { signal: this.location, defaultValue: undefined },
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
      itemsSignal: this.roasters,
      currentPageSignal: this.currentPage,
      signalDefaults: this.signalDefaults,
      loadingKey: 'roaster-directory',
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: () =>
        this.roastersService.getRoasters({
          page: this.page(),
          pageSize: this.pageSize(),
          name: this.name(),
          location: this.location(),
        }),
    });
  }

  onNavigateAddRoaster() {
    this.router.navigate(['/roasters/create']);
  }

  onChangeName(event: Event) {
    this.name.set((event.target as HTMLInputElement).value);
  }

  onChangeLocation(event: Event) {
    this.location.set((event.target as HTMLInputElement).value);
  }

  onSearchRoaster() {
    if (!this.name() && !this.location()) return;
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

  get paginationText(): string {
    return getPaginationText(this.paginatedResultSignal());
  }
}
