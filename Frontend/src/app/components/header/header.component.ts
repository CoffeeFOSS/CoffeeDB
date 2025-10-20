import { Component, inject } from '@angular/core';
import { SidebarService } from '../../services/sidebar.service';
import { MenuButtonComponent } from '../sidebar/menu-button/menu-button.component';
import { UserHandleComponent } from '../user-handle/user-handle.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [UserHandleComponent, MenuButtonComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  public sidebar = inject(SidebarService);
  router = inject(Router);

  onOpenSidebar() {
    this.sidebar.open();
  }

  get role() {
    const { url } = this.router;
    if (url === '/admin' || url.startsWith('/admin/')) {
      return 'admin';
    }
    if (url === '/mod' || url.startsWith('/mod/')) {
      return 'mod';
    }
    return null;
  }
}
