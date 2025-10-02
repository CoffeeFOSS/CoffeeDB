import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = 0;
  private spinnerService = inject(NgxSpinnerService);

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'ball-pulse',
      bdColor: 'rgba(255, 255, 255, 0)',
      color: '#333333',
    });
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }

  isLoading() {
    return this.busyRequestCount > 1;
  }

  /**
   * Determines whether it's appropriate to show a "not found" message.
   * Returns true if loading is finished and the requested entity is explicitly null.
   * Prevents page flash by avoiding premature rendering during loading.
   */
  canShow<T>(dependency: T | null | undefined): boolean {
    return !this.isLoading() && dependency === null;
  }
}
