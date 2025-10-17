import { computed, Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  private activeRequests = signal(0);
  private activeOperations = new Set<string>();
  public readonly isLoading = computed(() => this.activeRequests() > 0);

  busy(id?: string) {
    if (id) {
      if (this.activeOperations.has(id)) return;
      this.activeOperations.add(id);
    }
    this.activeRequests.update((count) => count + 1);
  }

  idle(id?: string) {
    if (id) {
      if (!this.activeOperations.has(id)) return;
      this.activeOperations.delete(id);
    }
    this.activeRequests.update((count) => count - 1);
    if (this.activeRequests() <= 0) {
      this.activeRequests.set(0);
    }
  }

  isLoadingId(id: string) {
    return this.activeOperations.has(id);
  }

  /**
   * Determines whether it's appropriate to show a "not found" message.
   * Returns true if loading is finished and the requested entity is explicitly null.
   * Prevents page flash by avoiding premature rendering during loading.
   */
  canShow<T>(dependency: T | null | undefined): boolean {
    return !this.isLoading && dependency === null;
  }
}
