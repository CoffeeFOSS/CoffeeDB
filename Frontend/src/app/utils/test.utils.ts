import { HttpTestingController } from '@angular/common/http/testing';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export function expectHttpSignalUpdate<T extends Object>(
  httpMock: HttpTestingController,
  observable: Observable<T>,
  endpoint: string,
  flushedValue: T,
) {
  const baseUrl = environment.apiUrl;
  let result: T | undefined;

  observable.subscribe((res) => {
    result = res;
  });

  const req = httpMock.expectOne(baseUrl + endpoint);
  req.flush(flushedValue);

  expect(result).toEqual(flushedValue);
}
