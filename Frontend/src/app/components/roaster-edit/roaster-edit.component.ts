import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Roaster } from '../../models/roaster';
import { LoadingService } from '../../services/loading.service';
import { RoastersService } from '../../services/roasters.service';

@Component({
  selector: 'app-roaster-edit',
  imports: [],
  templateUrl: './roaster-edit.component.html',
  styleUrl: './roaster-edit.component.scss',
})
export class RoasterEditComponent implements OnInit {
  private roastersService = inject(RoastersService);
  private route = inject(ActivatedRoute);
  private id: number | null = null;
  loadingService = inject(LoadingService);
  roaster?: Roaster;

  ngOnInit(): void {
    this.loadRoaster();
  }

  loadRoaster() {
    const roasterId = Number(this.route.snapshot.paramMap.get('id'));
    if (roasterId === undefined || isNaN(roasterId)) return;
    this.id = roasterId;

    this.roastersService.getRoaster(roasterId).subscribe({
      next: (roaster: Roaster) => {
        this.roaster = roaster;
      },
    });
  }
}
