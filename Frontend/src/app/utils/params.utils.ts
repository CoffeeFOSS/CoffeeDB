import { HttpParams, HttpResponse } from '@angular/common/http';
import { signal, WritableSignal } from '@angular/core';
import { QUERY_PARAMS } from '../constants/query.constants';
import { ActivatedRoute, Router } from '@angular/router';
import { PaginatedResult, PaginationSignals } from '../models/pagination';
import { LoadingService } from '../services/loading.service';
import { Observable } from 'rxjs';
import { getPaginatedResult } from './pagination.utils';
import { FormGroup, ValidatorFn } from '@angular/forms';
import { setSubmittedAndValidateForm } from './form.utils';

export type FormKeyMap = Record<
  string,
  { paramCode: string; default: any; validators?: ValidatorFn[] }
>;

export function getHttpParams(model: any) {
  let params = new HttpParams();

  for (const key in model) {
    if (model[key]) params = params.append(key, model[key]);
  }

  return params;
}

export function extractAndSetParams(
  urlParams: Record<string, any>,
  formGroup: FormGroup,
  paginationSignals: PaginationSignals,
  formKeyMap: FormKeyMap,
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

  for (const [_, { signal, defaultValue }] of paginationEntries) {
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

  const allAtDefault = !activePaginationEntries.length && !activeEntries.length;

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

function safeNullCheck(value: any) {
  return value !== undefined && value !== null && value !== '';
}

export function fetchItemsWithCache<T>({
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
  const queryKey = getQueryKey([String(page), String(pageSize)], params);

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

// Careful: constructs the queryKey in the order of the keys params!
function getQueryKey(initialArray: string[], params: Record<string, any>) {
  const keyBuilder: string[] = [...initialArray];
  for (const key of Object.keys(params)) {
    if (safeNullCheck(params[key])) keyBuilder.push(`${key}=${params[key]}`);
  }

  return keyBuilder.join('&');
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
  formKeyMap: FormKeyMap,
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
  formKeyMap: FormKeyMap;
  submittedSignal: WritableSignal<boolean>;
  validationErrors: string[];
}) {
  return route.queryParams.subscribe((urlParams) => {
    extractAndSetParams(urlParams, formGroup, paginationSignals, formKeyMap);

    if (!formGroup.dirty) return;
    setSubmittedAndValidateForm(submittedSignal, formGroup, validationErrors);
  });
}

export function buildParamsFromForm(
  formValues: Record<string, any>,
  formKeyMap: FormKeyMap,
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
