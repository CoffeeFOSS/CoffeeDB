import {
  Component,
  effect,
  inject,
  OnInit,
  signal,
  untracked,
} from '@angular/core';
import {
  buildParamsFromForm,
  buildParamsValueDefaults,
  createPaginationSignals,
  fetchItemsWithCache,
  sanitizeObjectFields,
  subscribeToQueryParams,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { RoastersService } from '../../services/roasters.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthDirective } from '../../directive/auth.directive';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { LoadingService } from '../../services/loading.service';
import { Roaster } from '../../models/roaster';
import { PaginatedResult } from '../../models/pagination';
import { getPaginationText } from '../../utils/pagination.utils';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../forms/text-input/text-input.component';
import {
  allControlsGroupFilled,
  requireAllControlsValidator,
  setSubmittedAndValidateForm,
} from '../../utils/form.utils';
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
export class RoasterDirectoryComponent implements OnInit {
  private roastersService = inject(RoastersService);
  router = inject(Router);
  private route = inject(ActivatedRoute);
  private cache: Record<string, PaginatedResult<Roaster[]>> = {};
  private fb = new FormBuilder();
  loadingService = inject(LoadingService);
  searchForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  paginationSignals = createPaginationSignals();
  roasters = signal<Roaster[]>([]);
  paginatedResultSignal = signal<PaginatedResult<Roaster[]> | null>(null);
  submitted = signal(false);

  private coordinateControlNames = ['lat', 'long', 'radius'];
  private formKeyMap: Record<string, { paramCode: string; default: any }> = {
    name: { paramCode: 'n', default: '' },
    address: { paramCode: 'a', default: '' },
    lat: { paramCode: 'la', default: '' },
    long: { paramCode: 'lo', default: '' },
    radius: { paramCode: 'r', default: '' },
  };

  constructor() {
    effect(() => {
      this.paginationSignals.page.signal();
      this.paginationSignals.pageSize.signal();
      untracked(() => {
        this.syncParamsWithUrl();
        this.fetchItems();
      });
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    subscribeToQueryParams({
      route: this.route,
      formGroup: this.searchForm,
      paginationSignals: this.paginationSignals,
      formKeyMap: this.formKeyMap,
      submittedSignal: this.submitted,
      validationErrors: this.validationErrors,
    });
  }

  initializeForm() {
    const { name, address, lat, long, radius } = this.formKeyMap;
    this.searchForm = this.fb.group(
      {
        name: [name.default, [Validators.maxLength(100)]],
        address: [address.default, [Validators.maxLength(200)]],
        lat: [lat.default, [Validators.min(-90), Validators.max(90)]],
        long: [long.default, [Validators.min(-180), Validators.max(180)]],
        radius: [radius.default, [Validators.min(0.1), Validators.max(15000)]],
      },
      {
        validators: requireAllControlsValidator(this.coordinateControlNames),
      },
    );
  }

  fetchItems() {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.roasters,
      paginationSignals: this.paginationSignals,
      params: buildParamsFromForm(this.searchForm.value, this.formKeyMap),
      loadingKey: 'roaster-directory',
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: () =>
        this.roastersService.getRoasters({
          page: this.paginationSignals.page.signal(),
          pageSize: this.paginationSignals.pageSize.signal(),
          ...sanitizeObjectFields(this.searchForm.value),
        }),
    });
  }

  syncParamsWithUrl() {
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      paginationSignals: this.paginationSignals,
      params: buildParamsValueDefaults(this.searchForm.value, this.formKeyMap),
    });
  }

  onSearchSubmit() {
    setSubmittedAndValidateForm(
      this.submitted,
      this.searchForm,
      this.validationErrors,
    );
    this.syncParamsWithUrl();
    this.fetchItems();
  }

  onResetSearch() {
    this.searchForm.reset();
    this.onSearchSubmit();
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
    return allControlsGroupFilled(this.searchForm, this.coordinateControlNames);
  }
}
