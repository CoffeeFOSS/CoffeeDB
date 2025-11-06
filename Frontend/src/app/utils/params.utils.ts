import { HttpParams, HttpResponse } from '@angular/common/http';
import { signal, WritableSignal } from '@angular/core';
import { QUERY_PARAMS } from '../constants/query.constants';
import { ActivatedRoute, Router } from '@angular/router';
import { PaginatedResult, PaginationSignals } from '../models/pagination';
import { LoadingService } from '../services/loading.service';
import { Observable } from 'rxjs';
import { getPaginatedResult } from './pagination.utils';
import { FormGroup } from '@angular/forms';
import { setSubmittedAndValidateForm } from './form.utils';

export function getHttpParams(model: any) {
  let params = new HttpParams();

  for (const key in model) {
    if (model[key]) params = params.append(key, model[key]);
  }

  return params;
}

export interface SignalDefault<T> {
  signal: WritableSignal<any>;
  defaultValue: T;
  unionId?: string;
}

export interface SearchableSignalDefault<T> extends SignalDefault<T> {
  searchLabel: string;
}

export function extractAndSetParams(
  params: Record<string, any>,
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  for (const key of Object.keys(signalDefaults)) {
    const { signal, defaultValue } = signalDefaults[key];

    if (!(key in params)) {
      signal.set(defaultValue);
      continue;
    }

    switch (key) {
      case 'p':
        const pageParam = Number(params['p']);
        if (!isNaN(pageParam) && pageParam > 0 && signal() != pageParam) {
          signal.set(pageParam);
        }
        break;

      case 's':
        let pageSizeParam = Number(params['s']);
        if (!isNaN(pageSizeParam) && signal() != pageSizeParam) {
          pageSizeParam = Math.max(QUERY_PARAMS.PAGE_SIZE.MIN, pageSizeParam);
          pageSizeParam = Math.min(QUERY_PARAMS.PAGE_SIZE.MAX, pageSizeParam);
          signal.set(pageSizeParam);
        }
        break;

      default:
        const param = String(params[key]);
        if (param) {
          signal.set(param);
        }
    }
  }
}

export function extractAndSetParamsNew(
  urlParams: Record<string, any>,
  formGroup: FormGroup,
  paginationSignals: PaginationSignals,
  formKeyMap: Record<string, { paramCode: string; default: any }>,
) {
  const pageParam = Number(urlParams['p']);
  if (!isNaN(pageParam) && pageParam > 0) {
    paginationSignals.page.signal.set(pageParam);
  }

  let pageSizeParam = Number(urlParams['s']);
  if (!isNaN(pageSizeParam)) {
    pageSizeParam = Math.max(QUERY_PARAMS.PAGE_SIZE.MIN, pageSizeParam);
    pageSizeParam = Math.min(QUERY_PARAMS.PAGE_SIZE.MAX, pageSizeParam);
    paginationSignals.pageSize.signal.set(pageSizeParam);
  }

  for (const [formKey, { paramCode }] of Object.entries(formKeyMap)) {
    const control = formGroup.get(formKey);
    if (!control) continue;

    const paramValue = urlParams[paramCode];
    if (paramValue == null || paramValue === '') continue;

    control.setValue(String(paramValue));
    formGroup.markAsDirty();
  }
}

export function syncParamsWithUrl({
  router,
  route,
  signalDefaults,
}: {
  router: Router;
  route: ActivatedRoute;
  signalDefaults: Record<string, SignalDefault<any>>;
}) {
  const entries = Object.entries(signalDefaults);

  for (const [_, { signal, defaultValue }] of entries) {
    if (signal() === undefined) signal.set(defaultValue);
  }

  const activeEntries = entries.filter(([key, { signal }]) => {
    const value = signal();
    return value !== null && value !== undefined && value !== '';
  });

  const allAtDefault = activeEntries.every(
    ([_, { signal, defaultValue }]) => signal() === defaultValue,
  );

  router.navigate([], {
    relativeTo: route,
    queryParams: allAtDefault
      ? {}
      : Object.fromEntries(entries.map(([key, { signal }]) => [key, signal()])),
    replaceUrl: true,
  });
}

export function syncParamsWithUrlNew({
  router,
  route,
  paginationSignals,
  params,
}: {
  router: Router;
  route: ActivatedRoute;
  paginationSignals: PaginationSignals;
  params: Record<string, { value: any; defaultValue: any }>;
}) {
  const paginationEntries = Object.entries(paginationSignals);
  const entries = Object.entries(params);

  for (const [key, { signal, defaultValue }] of paginationEntries) {
    if (signal() === undefined || signal() === null) signal.set(defaultValue);
  }
  for (const [key, { value, defaultValue }] of entries) {
    if (value === undefined || value === '' || value === null)
      params[key].value = defaultValue;
  }

  const activeEntries = entries.filter(
    ([_, { value, defaultValue }]) => value !== defaultValue,
  );
  const hasActiveEntries = activeEntries.length > 0;
  const activePaginationEntries = paginationEntries.filter(
    ([key, { signal, defaultValue }]) => {
      if (key === 'p') return signal() !== defaultValue || hasActiveEntries;
      if (key === 's')
        return (
          paginationSignals.page.signal() !==
            paginationSignals.page.defaultValue ||
          paginationSignals.pageSize.signal() !==
            paginationSignals.pageSize.defaultValue ||
          hasActiveEntries
        );
      return false;
    },
  );

  const allAtDefault =
    activePaginationEntries.length === 0 && activeEntries.length === 0;

  router.navigate([], {
    relativeTo: route,
    queryParams: allAtDefault
      ? {}
      : {
          ...Object.fromEntries(
            activePaginationEntries.map(([key, { signal }]) => [key, signal()]),
          ),
          ...Object.fromEntries(
            activeEntries.map(([key, { value }]) => [key, value]),
          ),
        },
    replaceUrl: true,
  });
}

// Careful: constructs the queryKey in the order keys are added to the signalDefaults!
export function getQueryKey(
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  return Object.entries(signalDefaults)
    .map(([key, { signal }]) => `${key}=${signal() ?? ''}`)
    .join('&');
}

export function fetchItemsWithCache<T>({
  cache,
  itemsSignal,
  currentPageSignal,
  signalDefaults,
  loadingKey,
  loadingService,
  resultSignal,
  fetchPaginatedItems,
}: {
  cache: Record<string, PaginatedResult<T[]>>;
  itemsSignal: WritableSignal<T[]>;
  currentPageSignal: WritableSignal<number | string>;
  signalDefaults: Record<string, SignalDefault<any>>;
  loadingKey: string;
  loadingService: LoadingService;
  resultSignal: WritableSignal<PaginatedResult<T[]> | null>;
  fetchPaginatedItems: () => Observable<HttpResponse<T[]>>;
}) {
  const queryKey = getQueryKey(signalDefaults);

  if (cache[queryKey]) {
    const current = itemsSignal();
    current.splice(0, current.length, ...cache[queryKey].items!);
    currentPageSignal.set(
      signalDefaults['p']?.signal() ?? QUERY_PARAMS.PAGE.DEFAULT,
    );
    resultSignal.set(cache[queryKey]);
    return;
  }

  loadingService.busy(loadingKey);
  fetchPaginatedItems().subscribe({
    next: (res: HttpResponse<T[]>) => {
      loadingService.idle(loadingKey);
      const result = getPaginatedResult(res);
      cache[queryKey] = result;
      const current = itemsSignal();
      current.splice(0, current.length, ...result.items!);
      currentPageSignal.set(
        signalDefaults['p']?.signal() ?? QUERY_PARAMS.PAGE.DEFAULT,
      );
      resultSignal.set(result);
    },
    error: () => loadingService.idle(loadingKey),
  });
}

function safeNullCheck(value: any) {
  return value !== undefined && value !== null && value !== '';
}

export function fetchItemsWithCacheNew<T>({
  cache,
  itemsSignal,
  paginationSignals,
  params,
  loadingKey,
  loadingService,
  resultSignal,
  fetchPaginatedItems,
}: {
  cache: Record<string, PaginatedResult<T[]>>;
  itemsSignal: WritableSignal<T[]>;
  paginationSignals: PaginationSignals;
  params: Record<string, any>;
  loadingKey: string;
  loadingService: LoadingService;
  resultSignal: WritableSignal<PaginatedResult<T[]> | null>;
  fetchPaginatedItems: () => Observable<HttpResponse<T[]>>;
}) {
  const page = paginationSignals.page.signal();
  const pageSize = paginationSignals.pageSize.signal();
  const queryKey = getQueryKeyNew([String(page), String(pageSize)], params);

  if (cache[queryKey]) {
    const current = itemsSignal();
    current.splice(0, current.length, ...cache[queryKey].items!);
    paginationSignals.page.signal.set(page ?? QUERY_PARAMS.PAGE.DEFAULT);
    resultSignal.set(cache[queryKey]);
    return;
  }

  loadingService.busy(loadingKey);
  fetchPaginatedItems().subscribe({
    next: (res: HttpResponse<T[]>) => {
      loadingService.idle(loadingKey);
      const result = getPaginatedResult(res);
      cache[queryKey] = result;
      const current = itemsSignal();
      current.splice(0, current.length, ...result.items!);
      paginationSignals.page.signal.set(page ?? QUERY_PARAMS.PAGE.DEFAULT);
      resultSignal.set(result);
    },
    error: () => loadingService.idle(loadingKey),
  });
}

function getQueryKeyNew(initialArray: string[], params: Record<string, any>) {
  const keyBuilder: string[] = [...initialArray];

  for (const key of Object.keys(params)) {
    if (safeNullCheck(params[key])) keyBuilder.push(`${key}=${params[key]}`);
  }

  return keyBuilder.join('&');
}

export function resetSearchToSignalDefaults(
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  const entries = Object.entries(signalDefaults);
  for (const [_, { signal, defaultValue }] of entries) {
    signal.set(defaultValue);
  }
}

export function createPaginationSignals(
  pageSizeDefault?: number,
): PaginationSignals {
  return {
    page: {
      signal: signal(QUERY_PARAMS.PAGE.DEFAULT),
      defaultValue: QUERY_PARAMS.PAGE.DEFAULT,
    },
    pageSize: {
      signal: signal(pageSizeDefault ?? QUERY_PARAMS.PAGE_SIZE.DEFAULT),
      defaultValue: pageSizeDefault ?? QUERY_PARAMS.PAGE_SIZE.DEFAULT,
    },
  };
}

export function sanitizeObjectFields(obj: Record<string, any>) {
  for (const key of Object.keys(obj)) {
    if (!safeNullCheck(obj[key])) {
      delete obj[key];
    }
  }
  return obj;
}

export function buildParamsValueDefaults(
  formValues: Record<string, any>,
  formKeyMap: Record<string, { paramCode: string; default: any }>,
) {
  return Object.fromEntries(
    Object.entries(formKeyMap).map(
      ([formKey, { paramCode, default: defaultValue }]) => [
        paramCode,
        {
          value: formValues[formKey],
          defaultValue,
        },
      ],
    ),
  );
}

export function subscribeToQueryParams({
  route,
  formGroup,
  paginationSignals,
  formKeyMap,
  submittedSignal,
  validationErrors,
}: {
  route: ActivatedRoute;
  formGroup: FormGroup;
  paginationSignals: PaginationSignals;
  formKeyMap: Record<string, { paramCode: string; default: any }>;
  submittedSignal: WritableSignal<boolean>;
  validationErrors: string[];
}) {
  return route.queryParams.subscribe((urlParams) => {
    extractAndSetParamsNew(urlParams, formGroup, paginationSignals, formKeyMap);

    if (!formGroup.dirty) return;
    setSubmittedAndValidateForm(submittedSignal, formGroup, validationErrors);
  });
}

export function buildParamsFromForm(
  formValues: Record<string, any>,
  formKeyMap: Record<string, { paramCode: string; default: any }>,
) {
  const formKeys = Object.keys(formValues);
  const mapKeys = Object.keys(formKeyMap);

  const missingKeys = mapKeys.filter((key) => !formKeys.includes(key));
  if (missingKeys.length > 0) {
    console.error(
      `Form values are missing keys required by formKeyMap: ${missingKeys.join(', ')}`,
    );
  }

  return Object.fromEntries(
    Object.entries(formKeyMap).map(([formKey, { paramCode }]) => [
      paramCode,
      formValues[formKey],
    ]),
  );
}
