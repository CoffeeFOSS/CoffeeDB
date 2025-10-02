import { inject, Injectable, NgZone } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private spinnerService = inject(NgxSpinnerService);
  private ngZone = inject(NgZone);
  private activeRequests = 0;
  private startTime = 0;
  private readonly MIN_DISPLAY_TIME = 500;

  busy() {
    this.activeRequests++;
    if (this.activeRequests === 1) {
      this.ngZone.run(() => {
        this.spinnerService.show(undefined, {
          type: 'ball-pulse',
          bdColor: 'rgba(255, 255, 255, 0)',
          color: '#333333',
        });
      });
      this.startTime = Date.now();
    }
  }

  idle() {
    this.activeRequests--;
    if (this.activeRequests <= 0) {
      const elapsedTime = Date.now() - this.startTime;
      const delayTime = Math.max(0, this.MIN_DISPLAY_TIME - elapsedTime);
      this.activeRequests = 0;
      this.ngZone.run(() => {
        setTimeout(() => {
          this.spinnerService.hide();
        }, delayTime);
      });
    }
  }

  isLoading() {
    return this.activeRequests > 1;
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
