import { Component, input, output } from '@angular/core';
import { NgIcon } from '@ng-icons/core';
import { matMenuOutline } from '@ng-icons/material-icons/outline';

@Component({
  selector: 'app-menu-button',
  imports: [NgIcon],
  templateUrl: './menu-button.component.html',
  styleUrl: './menu-button.component.scss',
})
export class MenuButtonComponent {
  menuClick = output<void>();
  transparent = input<boolean>();
  menuIcon = matMenuOutline;
}
