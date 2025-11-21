import {
  effect,
  inject,
  Injectable,
  OnInit,
  signal,
  untracked,
  WritableSignal,
} from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PaginatedResult } from '../../../models/pagination';
import { LoadingService } from '../../../services/loading.service';
import {
  buildParamsFromForm,
  buildParamsValueDefaults,
  createPaginationSignals,
  fetchItemsWithCache,
  FormKeyMap,
  subscribeToQueryParams,
  syncParamsWithUrl,
} from '../../../utils/params.utils';
import { setSubmittedAndValidateForm } from '../../../utils/form.utils';
import { getPaginationText } from '../../../utils/pagination.utils';
import { HttpResponse } from '@angular/common/http';

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
    subscribeToQueryParams(
      this.route,
      this.searchForm,
      this.paginationSignals,
      this.formKeyMap,
      this.submitted,
      this.validationErrors,
    );
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

  protected fetchPaginatedItemsNext(
    res: HttpResponse<T[]>,
    result: PaginatedResult<T[]>,
  ): any {
    return null;
  }

  protected abstract fetchPaginatedItems(): any;

  fetchItems() {
    fetchItemsWithCache(
      this.cache,
      this.items,
      this.paginationSignals,
      buildParamsFromForm(this.searchForm.value, this.formKeyMap),
      this.loadingKey,
      this.loadingService,
      this.paginatedResultSignal,
      this.fetchPaginatedItems.bind(this),
      this.fetchPaginatedItemsNext.bind(this),
    );
  }

  syncParamsWithUrl() {
    syncParamsWithUrl(
      this.router,
      this.route,
      this.paginationSignals,
      buildParamsValueDefaults(this.searchForm.value, this.formKeyMap),
    );
  }

  onSearchSubmit() {
    const passedValidation = setSubmittedAndValidateForm(
      this.submitted,
      this.searchForm,
      this.validationErrors,
    );
    if (!passedValidation) return;
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
