import { Component, effect, inject, signal } from '@angular/core';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Router, ActivatedRoute } from '@angular/router';
import { QUERY_PARAMS } from '../../constants/query.constants';
import { PaginatedResult } from '../../models/pagination';
import { LoadingService } from '../../services/loading.service';
import {
  createGetPaginatedResult,
  createOnPageChange,
  createOnPageSizeChange,
  extractAndSetParams,
  fetchItemsWithCache,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import { Roaster } from '../../models/roaster';

@Component({
  selector: 'app-roasters',
  imports: [PaginationControlsComponent],
  templateUrl: './roasters.component.html',
  styleUrl: './roasters.component.scss',
})
export class RoastersComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  roasters = signal<Roaster[]>([]);
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Roaster[]>> = {};

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
        itemsSignal: this.roasters,
        currentPageSignal: this.currentPage,
        signalDefaults: this.signalDefaults,
        loadingKey: 'roasters',
        loadingService: this.loadingService,
        fetchPaginatedItems: () =>
          this.roastersService.getRoasters({
            page: this.page(),
            pageSize: this.pageSize(),
            name: undefined, // TODO
            location: undefined, // TODO
          }),
      });
    });
  }

  onPageChange = createOnPageChange(this.page);
  onPageSizeChange = createOnPageSizeChange(this.page, this.pageSize);

  get paginatedResult(): PaginatedResult<Roaster[]> | null {
    return createGetPaginatedResult<Roaster>(this.cache, this.signalDefaults);
  }

  onNavigateRoasterPage(id: number) {
    this.router.navigate(['/roasters', id]);
  }
}
