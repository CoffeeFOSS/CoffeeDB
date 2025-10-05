import { CommonModule } from '@angular/common';
import { Component, inject, input, output } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LinkButtonComponent } from './link-button/link-button.component';
import { SidebarService } from '../../services/sidebar.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, LinkButtonComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  accountService = inject(AccountService);
  sidebarService = inject(SidebarService);
}
