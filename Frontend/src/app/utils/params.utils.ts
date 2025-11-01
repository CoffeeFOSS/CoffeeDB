import { HttpParams, HttpResponse } from '@angular/common/http';
import { WritableSignal } from '@angular/core';
import { QUERY_PARAMS } from '../constants/query.constants';
import { ActivatedRoute, Router } from '@angular/router';
import { PaginatedResult } from '../models/pagination';
import { LoadingService } from '../services/loading.service';
import { Observable } from 'rxjs';
import { getPaginatedResult } from './pagination.utils';

export function getHttpParams(model: any) {
  let params = new HttpParams();

  for (const key in model) {
    if (model[key]) params = params.append(key, model[key]);
  }

  return params;
}

interface SignalDefault<T> {
  signal: WritableSignal<any>;
  defaultValue: T;
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

      case 'n':
        const nameParam = String(params['n']);
        if (nameParam) {
          signal.set(nameParam);
        }
        break;

      case 'l':
        const locationParam = String(params['l']);
        if (locationParam) {
          signal.set(locationParam);
        }
        break;

      default:
        console.error(`Unhandled extractAndSetParam key ${key}`);
    }
  }
}

interface QueryParamSyncConfig {
  router: Router;
  route: ActivatedRoute;
  signalDefaults: Record<string, SignalDefault<any>>;
}

export function syncParamsWithUrl({
  router,
  route,
  signalDefaults,
}: QueryParamSyncConfig) {
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

// Careful: constructs the queryKey in the order keys are added to the signalDefaults!
export function getQueryKey(
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  return Object.entries(signalDefaults)
    .map(([key, { signal }]) => `${key}=${signal() ?? ''}`)
    .join('&');
}

interface FetchWithCacheConfig<T> {
  cache: Record<string, PaginatedResult<T[]>>;
  itemsSignal: WritableSignal<T[]>;
  currentPageSignal: WritableSignal<number | string>;
  signalDefaults: Record<string, SignalDefault<any>>;
  loadingKey: string;
  loadingService: LoadingService;
  resultSignal: WritableSignal<PaginatedResult<T[]> | null>;
  fetchPaginatedItems: () => Observable<HttpResponse<T[]>>;
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
}: FetchWithCacheConfig<T>) {
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

export function resetSearchToSignalDefaults(
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  const entries = Object.entries(signalDefaults);
  for (const [_, { signal, defaultValue }] of entries) {
    signal.set(defaultValue);
  }
}
