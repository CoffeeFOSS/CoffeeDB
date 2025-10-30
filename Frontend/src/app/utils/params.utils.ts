import { HttpParams } from '@angular/common/http';
import { Signal, WritableSignal } from '@angular/core';
import { QUERY_PARAMS } from '../constants/query.constants';
import { ActivatedRoute, Router } from '@angular/router';

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
  initialPageLoad: { value: boolean };
}

export function syncParamsWithUrl({
  router,
  route,
  signalDefaults,
  initialPageLoad,
}: QueryParamSyncConfig) {
  const entries = Object.entries(signalDefaults);

  if (initialPageLoad) {
    for (const [key, { signal, defaultValue }] of entries) {
      if (signal() === undefined) signal.set(defaultValue);
    }
  }

  const allAtDefault = entries.every(
    ([_, { signal, defaultValue }]) => signal() === defaultValue,
  );

  if (initialPageLoad && allAtDefault) {
    initialPageLoad.value = false;
    return;
  }

  router.navigate([], {
    relativeTo: route,
    queryParams: Object.fromEntries(
      entries.map(([key, { signal }]) => [key, signal()]),
    ),
  });
}
