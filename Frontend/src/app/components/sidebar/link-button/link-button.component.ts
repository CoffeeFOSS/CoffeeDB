import { Component, inject, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarService } from '../../../services/sidebar.service';
import { NgIcon } from '@ng-icons/core';

@Component({
  selector: 'app-link-button',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgIcon],
  templateUrl: './link-button.component.html',
  styleUrl: './link-button.component.scss',
})
export class LinkButtonComponent {
  sidebarService = inject(SidebarService);
  routerLink = input.required<string>();
  icon = input<string>();
}
