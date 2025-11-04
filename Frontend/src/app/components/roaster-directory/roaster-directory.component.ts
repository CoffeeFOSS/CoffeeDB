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
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import { requireOtherControlValidator } from '../../utils/form.utils';
import { ErrorTextComponent } from "../error-text/error-text.component";

@Component({
  selector: 'app-roasters',
  imports: [
    PaginationControlsComponent,
    AuthDirective,
    RouterLink,
    TextInputComponent,
    ReactiveFormsModule,
    ErrorTextComponent
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
    this.route.queryParams.subscribe(
      (params) => {},
      // extractAndSetParams(params, this.signalDefaults),
      // need another way to extract and set params
    );

    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        // syncParamsWithUrl({
        //   router: this.router,
        //   route: this.route,
        //   signalDefaults: this.signalDefaults,
        // });
      });
    });

    effect(() => {
      this.page();
      this.pageSize();
      untracked(() => {
        // this.fetchItemsTrigger();
      });
    });
  }

  ngOnInit(): void {
    // Extract query params
    this.route.queryParams.subscribe(
      (params) => {},
      // extractAndSetParams(params, this.signalDefaults),
    );

    // Trigger fetch on form changes (DONT DO THIS)
    this.searchForm.valueChanges.subscribe(() => {
      // this.page = QUERY_PARAMS.PAGE.DEFAULT;
      // this.fetchItems();
      // this.syncUrlParams();
    });

    this.initializeForm();
  }

  initializeForm() {
    this.searchForm = this.fb.group(
      {
        name: ['', [Validators.maxLength(100)]],
        locationAddress: ['', [Validators.maxLength(200)]],
        latitude: ['', [Validators.min(-90), Validators.max(90)]],
        longitude: ['', [Validators.min(-180), Validators.max(180)]],
        radius: ['', [Validators.min(0.1), Validators.max(180)]],
      },
      {
        validators: requireOtherControlValidator(this.coordinateSearchGroup),
      },
    );
  }

  fetchItemsTrigger() {
    // fetchItemsWithCache({
    //   cache: this.cache,
    //   itemsSignal: this.roasters,
    //   currentPageSignal: this.currentPage,
    //   signalDefaults: this.signalDefaults,
    //   loadingKey: 'roaster-directory',
    //   loadingService: this.loadingService,
    //   resultSignal: this.paginatedResultSignal,
    //   fetchPaginatedItems: () =>
    //     this.roastersService.getRoasters({
    //       page: this.page(),
    //       pageSize: this.pageSize(),
    //       name: this.name(),
    //       address: this.locationAddress(),
    //       lat: this.latitude() ?? undefined,
    //       long: this.longitude() ?? undefined,
    //       radius: this.radius() ?? undefined,
    //     }),
    // });
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
    console.log(this.searchForm.value);
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
