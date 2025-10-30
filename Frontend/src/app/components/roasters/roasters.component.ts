import { Component, effect, inject, signal } from '@angular/core';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Router, ActivatedRoute } from '@angular/router';
import { QUERY_PARAMS } from '../../constants/query.constants';
import { PaginatedResult } from '../../models/pagination';
import { LoadingService } from '../../services/loading.service';
import { getPaginatedResult } from '../../utils/pagination.utils';
import {
  extractAndSetParams,
  getQueryKey,
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
  roastersService = inject(RoastersService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);

  roasters = signal<Roaster[]>([]);
  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  signalDefaults = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
  };

  private cache: Record<string, PaginatedResult<Roaster[]>> = {};

  constructor() {
    const { router, route, signalDefaults } = this;
    this.route.queryParams.subscribe((params) =>
      extractAndSetParams(params, this.signalDefaults),
    );
    effect(() => syncParamsWithUrl({ router, route, signalDefaults }));
  }

  fetchRoastersEffect = effect(() => {
    const queryKey = getQueryKey(this.signalDefaults);

    if (this.cache[queryKey]) {
      const current = this.roasters();
      current.splice(0, current.length, ...this.cache[queryKey].items!);
      this.currentPage.set(this.page());
      return;
    }

    this.loadingService.busy('roasters');
    const roastersSearchParams = {
      page: this.page(),
      pageSize: this.pageSize(),
      name: undefined, // TODO
      location: undefined, // TODO
    };
    this.roastersService.getRoasters(roastersSearchParams).subscribe({
      next: (res) => {
        this.loadingService.idle('roasters');
        const result = getPaginatedResult(res);
        this.cache[queryKey] = result;
        const current = this.roasters();
        current.splice(0, current.length, ...result.items!);
        this.currentPage.set(this.page());
      },
      error: () => this.loadingService.idle('roasters'),
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

  get paginatedResult(): PaginatedResult<Roaster[]> | null {
    const queryKey = getQueryKey(this.signalDefaults);
    return this.cache[queryKey] || null;
  }
}
