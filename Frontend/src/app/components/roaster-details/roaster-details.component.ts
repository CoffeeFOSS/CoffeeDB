import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { RoastersService } from '../../services/roasters.service';
import { Roaster } from '../../models/roaster';

@Component({
  selector: 'app-roaster-details',
  imports: [],
  templateUrl: './roaster-details.component.html',
  styleUrl: './roaster-details.component.scss',
})
export class RoasterDetailsComponent {
  private roastersService = inject(RoastersService);
  private route = inject(ActivatedRoute);
  loadingService = inject(LoadingService);
  roaster?: Roaster;

  ngOnInit(): void {
    this.loadRoaster();
  }

  loadRoaster() {
    const roasterId = Number(this.route.snapshot.paramMap.get('id'));
    if (roasterId === undefined || isNaN(roasterId)) return;

    this.roastersService.getRoaster(roasterId).subscribe({
      next: (roaster: Roaster) => {
        this.roaster = roaster;
      },
    });
  }
}
