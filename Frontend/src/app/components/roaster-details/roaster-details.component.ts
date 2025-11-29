import { Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { RoastersService } from '../../services/roasters.service';
import { Roaster } from '../../models/roaster';
import { RoastersFrameService } from '../../services/roaster-frame.service';

@Component({
  selector: 'app-roaster-details',
  imports: [],
  templateUrl: './roaster-details.component.html',
  styleUrl: './roaster-details.component.scss',
})
export class RoasterDetailsComponent {
  private roastersService = inject(RoastersService);
  roastersFrameService = inject(RoastersFrameService);
  private router = inject(Router);
  private id: number | null = null;
  loadingService = inject(LoadingService);
  loadingKey = '';

  constructor() {
    this.loadingKey = `roasters-${this.roastersFrameService.roaster()?.id}`;

    effect(() => {
      const roasterId = this.roastersFrameService.roasterId();
      const roaster = this.roastersFrameService.roaster();

      if (roasterId && !roaster) {
        this.loadingService.busy(this.loadingKey);
        this.roastersService.getRoaster(roasterId).subscribe({
          next: (roaster: Roaster) => {
            this.roastersFrameService.roaster.set(roaster);
            this.loadingService.idle(this.loadingKey);
          },
          error: () => {
            this.loadingService.idle(this.loadingKey);
          },
        });
      }
    });
  }

  onNavigateAddRoaster() {
    if (this.id == null) return;
    this.router.navigate(['/roasters', this.id, 'edit']);
  }

  get descriptionParagraphs(): string[] {
    const roaster = this.roastersFrameService.roaster();
    if (!roaster?.description) {
      return [];
    }
    return roaster.description.split('\n').filter((p) => p.trim().length > 0);
  }
}
