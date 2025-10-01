import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
// import { SidebarService } from '../../services/sidebar.service';
// import { UserMenuComponent } from '../user-menu/user-menu.component';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-header',
  standalone: true,
  // imports: [FormsModule, UserMenuComponent], // TODO: Change to reactive forms later on
  imports: [FormsModule], // TODO: Change to reactive forms later on
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  accountService = inject(AccountService);
  userMenuOpen = false;

  constructor(
    private router: Router,
    // public sidebar: SidebarService,
  ) {}

  onOpenSidebar() {
    // this.sidebar.open();
  }

  onSignInClick() {
    this.router.navigate(['/signin'], {
      queryParams: { redirectUrl: this.router.url },
    });
  }

  onSignOutClick() {
    this.accountService.signOut();
    this.router.navigateByUrl('/');
  }

  onUserMenuClick() {
    this.userMenuOpen = !this.userMenuOpen;
  }
}
