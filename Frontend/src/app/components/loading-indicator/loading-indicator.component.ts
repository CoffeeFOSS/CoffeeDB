import { Component, effect, inject, signal } from '@angular/core';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-loading-indicator',
  standalone: true, // Assuming this is missing in your snippet
  imports: [],
  templateUrl: './loading-indicator.component.html',
  styleUrl: './loading-indicator.component.scss',
})
export class LoadingIndicatorComponent {
  loadingService = inject(LoadingService);
  progress = signal(0);
  visible = signal(false);
  private intervalId: any;
  private resetTimeoutId: any;

  loadingEffect = effect(() => {
    if (this.loadingService.isLoading()) {
      this.startProgress();
    } else {
      this.completeProgress();
    }
  });

  private startProgress() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
    if (this.resetTimeoutId) {
      clearTimeout(this.resetTimeoutId);
      this.resetTimeoutId = null;
    }
    this.visible.set(true);
    this.progress.set(0);

    // Go from 0% → 85% in ~1000ms
    this.intervalId = setInterval(() => {
      const progress = this.progress();
      if (progress < 0.85) {
        this.progress.set(progress + 0.01);
      } else {
        clearInterval(this.intervalId);
        this.intervalId = null;
      }
    }, 12);
  }

  private completeProgress() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }

    this.progress.set(1);
    this.resetTimeoutId = setTimeout(() => {
      this.visible.set(false);
      this.resetTimeoutId = setTimeout(() => {
        this.progress.set(0);
        this.resetTimeoutId = null;
      }, 200); // wait for opacity transition
    }, 200);
  }
}
