import { Component, effect, inject, signal, untracked } from '@angular/core';
import {
  extractAndSetParamsNew,
  fetchItemsWithCacheNew,
  syncParamsWithUrlNew,
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
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { requireOtherControlValidator } from '../../utils/form.utils';
import { ErrorTextComponent } from '../error-text/error-text.component';

@Component({
  selector: 'app-roasters',
  imports: [
    PaginationControlsComponent,
    AuthDirective,
    RouterLink,
    TextInputComponent,
    ReactiveFormsModule,
    ErrorTextComponent,
  ],
  templateUrl: './roaster-directory.component.html',
  styleUrl: './roaster-directory.component.scss',
})
export class RoasterDirectoryComponent {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(HotToastService);
  loadingService = inject(LoadingService);

  searchForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];

  page = signal(QUERY_PARAMS.PAGE.DEFAULT);
  pageSize = signal(QUERY_PARAMS.PAGE_SIZE.DEFAULT);
  roasters = signal<Roaster[]>([]);
  paginatedResultSignal = signal<PaginatedResult<Roaster[]> | null>(null);
  private currentPage = signal(QUERY_PARAMS.PAGE.DEFAULT);
  private cache: Record<string, PaginatedResult<Roaster[]>> = {};
  private fb = new FormBuilder();
  coordinateSearchGroup = ['latitude', 'longitude', 'radius'];

  submitted = false;

  // name = signal('');
  // locationAddress = signal('');
  // longitude = signal<number | null>(null);
  // latitude = signal<number | null>(null);
  // radius = signal<number | null>(null);

  // signalDefaults: Record<string, SignalDefault<any>> = {
  //   p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
  //   s: { signal: this.pageSize, defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT },
  //   n: { signal: null, defaultValue: null },
  //   a: { signal: this.locationAddress, defaultValue: null },
  //   la: { signal: this.latitude, defaultValue: null },
  //   lo: { signal: this.longitude, defaultValue: null },
  //   r: { signal: this.radius, defaultValue: null },
  // };

  constructor() {
    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        this.syncParamsWithUrl();
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

  mapper = {
    name: 'n',
    locationAddress: 'a',
    latitude: 'la',
    longitude: 'lo',
    radius: 'r',
  };

  ngOnInit(): void {
    this.initializeForm();

    // Extract query params
    this.route.queryParams.subscribe((params) => {
      extractAndSetParamsNew(
        params,
        this.searchForm,
        this.mapper,
        this.page,
        this.pageSize,
      );
      this.submitted = true;
      if (!this.searchForm.valid) {
        this.searchForm.markAllAsTouched();
        this.validationErrors = [
          'At least one field was not provided correctly.',
        ];
        return;
      }
    });
  }

  initializeForm() {
    this.searchForm = this.fb.group(
      {
        name: ['', [Validators.maxLength(100)]],
        locationAddress: ['', [Validators.maxLength(200)]],
        latitude: ['', [Validators.min(-90), Validators.max(90)]],
        longitude: ['', [Validators.min(-180), Validators.max(180)]],
        radius: ['', [Validators.min(0.1), Validators.max(15000)]],
      },
      {
        validators: requireOtherControlValidator(this.coordinateSearchGroup),
      },
    );
  }

  fetchItemsTrigger() {
    const { name, locationAddress, latitude, longitude, radius } =
      this.searchForm.value;
    fetchItemsWithCacheNew({
      cache: this.cache,
      itemsSignal: this.roasters,
      pageSignal: this.page,
      pageSizeSignal: this.pageSize,
      currentPageSignal: this.currentPage,
      mapper: {
        n: name || undefined,
        a: locationAddress || undefined,
        la: latitude || undefined,
        lo: longitude || undefined,
        r: radius || undefined,
      },
      loadingKey: 'roaster-directory',
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: () =>
        this.roastersService.getRoasters({
          page: this.page(),
          pageSize: this.pageSize(),
          name: name || undefined,
          address: locationAddress || undefined,
          lat: latitude || undefined,
          long: longitude || undefined,
          radius: radius || undefined,
        }),
    });
  }

  onSearchSubmit() {
    this.submitted = true;
    if (!this.searchForm.valid) {
      this.searchForm.markAllAsTouched();
      this.validationErrors = [
        'At least one field was not provided correctly.',
      ];
      return;
    }
    this.syncParamsWithUrl();
    this.fetchItemsTrigger();
  }

  syncParamsWithUrl() {
    const { name, locationAddress, latitude, longitude, radius } =
      this.searchForm.value;

    syncParamsWithUrlNew({
      router: this.router,
      route: this.route,
      paginationParams: {
        p: { signal: this.page, defaultValue: QUERY_PARAMS.PAGE.DEFAULT },
        s: {
          signal: this.pageSize,
          defaultValue: QUERY_PARAMS.PAGE_SIZE.DEFAULT,
        },
      },
      params: {
        n: { value: name, defaultValue: '' },
        a: { value: locationAddress, defaultValue: '' },
        la: { value: latitude, defaultValue: '' },
        lo: { value: longitude, defaultValue: '' },
        r: { value: radius, defaultValue: '' },
      },
    });
  }

  onNavigateCreate() {
    this.router.navigate(['/roasters/create']);
  }

  isSearchEnabled() {
    // if (!this.latitude() || !this.longitude() || !this.radius()) return false;
    return true;
  }

  onRefreshData() {
    this.fetchItemsTrigger();
    // syncParamsWithUrl({
    //   router: this.router,
    //   route: this.route,
    //   signalDefaults: this.signalDefaults,
    // });
  }

  onSearch() {
    // const searchValues = [this.latitude(), this.longitude(), this.radius()];
    // const requiredCount = searchValues.filter((b) => !!b).length;
    // if (requiredCount > 0 && requiredCount < 3) {
    //   this.toast.error(
    //     'Latitude, Longitude, and Radius must all be provided together.',
    //   );
    //   return;
    // }
    // this.page.set(QUERY_PARAMS.PAGE.DEFAULT);
    // this.onRefreshData();
  }

  onResetSearch() {
    // this.searchForm.reset();
    // resetSearchToSignalDefaults(this.signalDefaults);
    // this.onRefreshData();
  }

  get paginationText(): string {
    return getPaginationText(this.paginatedResultSignal());
  }

  get isDistanceProvided(): boolean {
    return this.roasters().some(
      (roaster) => roaster.distanceInKilometers !== null,
    );
  }

  get coordinateGroupHasError(): boolean {
    const controls = ['latitude', 'longitude', 'radius'].map((name) =>
      this.searchForm.get(name),
    );
    const anyFilled = controls.some((c) => !!c?.value);
    const anyEmpty = controls.some((c) => !c?.value);

    return anyFilled && anyEmpty;
  }
}
