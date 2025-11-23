import { HttpTestingController } from '@angular/common/http/testing';
import { Observable } from 'rxjs';

export function expectHttpSignalUpdate<T extends Object>(
  httpMock: HttpTestingController,
  observable: Observable<T>,
  endpoint: string,
  flushedValue: T,
) {
  let result: T | undefined;

  observable.subscribe((res) => {
    result = res;
  });

  const req = httpMock.expectOne(endpoint);
  req.flush(flushedValue);

  expect(result).toEqual(flushedValue);
}
