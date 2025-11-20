import { Component, inject, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';

@Component({
  selector: 'app-roaster-frame',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './roaster-frame.component.html',
  styleUrl: './roaster-frame.component.scss',
})
export class RoasterFrameComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  roasterId: number | null = null;

  ngOnInit() {
    const roasterId = Number(this.route.snapshot.paramMap.get('roasterId'));
    if (!isNaN(roasterId)) this.roasterId = roasterId;
  }
}
