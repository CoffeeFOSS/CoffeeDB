import {
  effect,
  inject,
  Injectable,
  OnInit,
  signal,
  untracked,
  WritableSignal,
} from '@angular/core';
import { FormGroup, FormBuilder, ValidatorFn } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PaginatedResult } from '../../../models/pagination';
import { LoadingService } from '../../../services/loading.service';
import {
  buildParamsFromForm,
  buildParamsValueDefaults,
  createPaginationSignals,
  fetchItemsWithCache,
  subscribeToQueryParams,
  syncParamsWithUrl,
} from '../../../utils/params.utils';
import { setSubmittedAndValidateForm } from '../../../utils/form.utils';
import { getPaginationText } from '../../../utils/pagination.utils';

export type FormKeyMap = Record<
  string,
  { paramCode: string; default: any; validators?: ValidatorFn[] }
>;

@Injectable()
export abstract class PaginatedDirectoryComponent<T, S> implements OnInit {
  protected abstract service: S;
  protected abstract items: WritableSignal<T[]>;
  protected abstract loadingKey: string;
  protected abstract formKeyMap: FormKeyMap;

  protected cache: Record<string, PaginatedResult<T[]>> = {};
  protected fb = new FormBuilder();
  protected route = inject(ActivatedRoute);
  protected router = inject(Router);

  loadingService = inject(LoadingService);
  searchForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  paginationSignals = createPaginationSignals();
  paginatedResultSignal = signal<PaginatedResult<T[]> | null>(null);
  submitted = signal(false);

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

  ngOnInit() {
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
    const controls: Record<string, any> = {};
    for (const key in this.formKeyMap) {
      const { default: defaultValue, validators } = this.formKeyMap[key];
      controls[key] = validators ? [defaultValue, validators] : [defaultValue];
    }
    this.searchForm = this.fb.group(controls, this.getFormGroupValidators());
  }

  protected getFormGroupValidators(): any {
    return null;
  }

  protected abstract fetchPaginatedItems(): any;

  fetchItems() {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.items,
      paginationSignals: this.paginationSignals,
      params: buildParamsFromForm(this.searchForm.value, this.formKeyMap),
      loadingKey: this.loadingKey,
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: this.fetchPaginatedItems,
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
}
