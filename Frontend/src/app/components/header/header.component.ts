import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarService } from '../../services/sidebar.service';
import { MenuButtonComponent } from '../sidebar/menu-button/menu-button.component';
import { UserHandleComponent } from '../user-handle/user-handle.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [FormsModule, UserHandleComponent, MenuButtonComponent], // TODO: Change to reactive forms later on
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
