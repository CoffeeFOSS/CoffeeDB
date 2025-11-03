import { Component, effect, inject, signal, untracked } from '@angular/core';
import {
  extractAndSetParams,
  fetchItemsWithCache,
  resetSearchToSignalDefaults,
  SignalDefault,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';
import { QUERY_PARAMS } from '../../constants/query.constants';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { LoadingService } from '../../services/loading.service';
import { Roaster } from '../../models/roaster';
import { PaginatedResult } from '../../models/pagination';
import { getPaginationText } from '../../utils/pagination.utils';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-roasters',
  imports: [PaginationControlsComponent, AuthDirective, RouterLink],
  templateUrl: './roaster-directory.component.html',
  styleUrl: './roaster-directory.component.scss',
})
export class RoasterDirectoryComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(HotToastService);
  loadingService = inject(LoadingService);
  roasters = signal<Roaster[]>([]);

  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  name = signal('');
  locationAddress = signal('');
  longitude = signal<number | null>(null);
  latitude = signal<number | null>(null);
  radius = signal<number | null>(null);

  paginatedResultSignal = signal<PaginatedResult<Roaster[]> | null>(null);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Roaster[]>> = {};

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
          locationAddress: this.locationAddress(),
          lat: this.latitude() ?? undefined,
          long: this.longitude() ?? undefined,
          radius: this.radius() ?? undefined,
        }),
    });
  }

  signalDefaults: Record<string, SignalDefault<any>> = {
    p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
    s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
    n: { signal: this.name, defaultValue: null },
    la: { signal: this.latitude, defaultValue: null },
    lo: { signal: this.longitude, defaultValue: null },
    r: { signal: this.radius, defaultValue: null },
  };

  fetchRoasters = (params: any) =>
    this.roastersService.getRoasters({
      page: params.p,
      pageSize: params.s,
      name: params.n,
      locationAddress: params.l,
      lat: params.la ?? undefined,
      long: params.lo ?? undefined,
      radius: params.r ?? undefined,
    });

  onNavigateCreate() {
    this.router.navigate(['/roasters/create']);
  }

  isSearchEnabled() {
    if (!this.latitude() || !this.longitude() || !this.radius()) return false;
    return true;
  }

  onRefreshData() {
    this.fetchItemsTrigger();
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      signalDefaults: this.signalDefaults,
    });
  }

  onSearch() {
    const searchValues = [this.latitude(), this.longitude(), this.radius()];
    const requiredCount = searchValues.filter((b) => !!b).length;

    if (requiredCount > 0 && requiredCount < 3) {
      this.toast.error(
        'Latitude, Longitude, and Radius must all be provided together.',
      );
      return;
    }
    this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    this.onRefreshData();
  }

  onResetSearch() {
    resetSearchToSignalDefaults(this.signalDefaults);
    this.onRefreshData();
  }

  get paginationText(): string {
    return getPaginationText(this.paginatedResultSignal());
  }

  get isDistanceProvided(): boolean {
    return this.roasters().some(
      (roaster) => roaster.distanceInKilometers !== null,
    );
  }
}
