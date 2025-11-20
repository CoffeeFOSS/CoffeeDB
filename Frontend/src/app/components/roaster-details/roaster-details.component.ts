import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { RoastersService } from '../../services/roasters.service';
import { Roaster } from '../../models/roaster';

@Component({
  selector: 'app-roaster-details',
  imports: [],
  templateUrl: './roaster-details.component.html',
  styleUrl: './roaster-details.component.scss',
})
export class RoasterDetailsComponent implements OnInit {
  private roastersService = inject(RoastersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private id: number | null = null;
  loadingService = inject(LoadingService);
  roaster?: Roaster;

  ngOnInit(): void {
    this.loadRoaster();
  }

  loadRoaster() {
    const roasterId = Number(
      this.route.parent?.snapshot.paramMap.get('roasterId'),
    );
    if (roasterId === undefined || isNaN(roasterId)) return;
    this.id = roasterId;

    this.roastersService.getRoaster(roasterId).subscribe({
      next: (roaster: Roaster) => {
        this.roaster = roaster;
      },
    });
  }

  onNavigateAddRoaster() {
    if (this.id == null) return;
    this.router.navigate(['/roasters', this.id, 'edit']);
  }

  get descriptionParagraphs(): string[] {
    if (!this.roaster?.description) {
      return [];
    }
    return this.roaster.description
      .split('\n')
      .filter((p) => p.trim().length > 0);
  }
}
