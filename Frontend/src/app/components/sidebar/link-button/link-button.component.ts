import { Component, inject, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarService } from '../../../services/sidebar.service';

@Component({
  selector: 'app-link-button',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './link-button.component.html',
  styleUrl: './link-button.component.scss',
})
export class LinkButtonComponent {
  sidebarService = inject(SidebarService);
  routerLink = input.required<string>();
}
