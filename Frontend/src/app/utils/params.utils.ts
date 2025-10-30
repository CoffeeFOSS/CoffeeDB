import { HttpParams, HttpResponse } from '@angular/common/http';
import { signal, WritableSignal } from '@angular/core';
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

export function extractAndSetParams<T>(
  params: Record<string, any>,
  signalDefaults: Record<string, SignalDefault<T>>,
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

  const allAtDefault = entries.every(
    ([_, { signal, defaultValue }]) => signal() === defaultValue,
  );

  router.navigate([], {
    relativeTo: route,
    queryParams: allAtDefault
      ? {}
      : Object.fromEntries(entries.map(([key, { signal }]) => [key, signal()])),
  });
}

// Careful: constructs the queryKey in the order keys are added to the signalDefaults!
export function getQueryKey(
  signalDefaults: Record<string, SignalDefault<any>>,
) {
  return Object.entries(signalDefaults)
    .map(([key, { signal }]) => `${key}=${signal()}`)
    .join('&');
}

interface FetchWithCacheConfig<T> {
  cache: Record<string, PaginatedResult<T[]>>;
  itemsSignal: WritableSignal<T[]>;
  currentPageSignal: WritableSignal<number>;
  signalDefaults: Record<string, SignalDefault<any>>;
  loadingKey: string;
  loadingService: LoadingService;
  fetchPaginatedItems: () => Observable<HttpResponse<T[]>>;
}

export function fetchItemsWithCache<T>({
  cache,
  itemsSignal,
  currentPageSignal,
  signalDefaults,
  loadingKey,
  loadingService,
  fetchPaginatedItems,
}: FetchWithCacheConfig<T>) {
  const queryKey = getQueryKey(signalDefaults);

  if (cache[queryKey]) {
    const current = itemsSignal();
    current.splice(0, current.length, ...cache[queryKey].items!);
    currentPageSignal.set(
      signalDefaults['p']?.signal() ?? QUERY_PARAMS.PAGE.DEFAULT,
    );
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
    },
    error: () => loadingService.idle(loadingKey),
  });
}
