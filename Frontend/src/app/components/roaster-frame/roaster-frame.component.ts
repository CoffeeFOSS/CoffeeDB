import { Component, inject, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { RoastersFrameService } from '../../services/roaster-frame.service';

@Component({
  selector: 'app-roaster-frame',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './roaster-frame.component.html',
  styleUrl: './roaster-frame.component.scss',
})
export class RoasterFrameComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  roastersFrameService = inject(RoastersFrameService);

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const roasterIdFromRoute = params.get('id');
      let roasterId =
        roasterIdFromRoute == null ? undefined : Number(roasterIdFromRoute);

      if (roasterId !== this.roastersFrameService.roaster()?.id) {
        this.roastersFrameService.reset();
        if (!isNaN(Number(roasterId))) {
          this.roastersFrameService.roasterId.set(Number(roasterId));
        } else if (roasterIdFromRoute == 'new') {
          this.roastersFrameService.roasterId.set(null);
        } else {
          console.error('shouldnt go here');
          return;
        }
      }
    });
  }
}
