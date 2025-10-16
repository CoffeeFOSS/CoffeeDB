import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LinkButtonComponent } from './link-button/link-button.component';
import { SidebarService } from '../../services/sidebar.service';
import {
  matAdminPanelSettingsOutline,
  matHomeOutline,
  matShieldOutline,
  matSquareOutline,
  matSupervisorAccountOutline,
} from '@ng-icons/material-icons/outline';
import { MenuButtonComponent } from './menu-button/menu-button.component';
import { HasRoleDirective } from '../../directives/has-role.directive';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    LinkButtonComponent,
    MenuButtonComponent,
    HasRoleDirective,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  accountService = inject(AccountService);
  sidebarService = inject(SidebarService);

  homeIcon = matHomeOutline;
  usersIcon = matSupervisorAccountOutline;
  shieldIcon = matShieldOutline;
  userManagerIcon = matAdminPanelSettingsOutline;
  placeholderIcon = matSquareOutline;
}
