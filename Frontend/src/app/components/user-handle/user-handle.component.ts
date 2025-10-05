import { Component, inject } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { Router } from '@angular/router';
import { UserMenuComponent } from "../user-menu/user-menu.component";

@Component({
  selector: 'app-user-handle',
  imports: [UserMenuComponent],
  templateUrl: './user-handle.component.html',
  styleUrl: './user-handle.component.scss',
})
export class UserHandleComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  userMenuOpen = false;

  onSignInClick() {
    this.router.navigate(['/signin'], {
      queryParams: { redirectUrl: this.router.url },
    });
  }

  onSignOutClick() {
    this.accountService.signOut();
    this.router.navigateByUrl('/');
  }

  onUserMenuClick($event: MouseEvent) {
    $event.stopPropagation();
    this.userMenuOpen = !this.userMenuOpen;
  }
}
