import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarService } from '../../services/sidebar.service';
import { UserHandleComponent } from '../user-handle/user-handle.component';
import { MenuButtonComponent } from '../sidebar/menu-button/menu-button.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [FormsModule, UserHandleComponent, MenuButtonComponent], // TODO: Change to reactive forms later on
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  public sidebar = inject(SidebarService);

  onOpenSidebar() {
    this.sidebar.open();
  }
}
