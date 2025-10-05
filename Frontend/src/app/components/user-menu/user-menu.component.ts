import {
  Component,
  ElementRef,
  HostListener,
  inject,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss',
})
export class UserMenuComponent {
  accountService = inject(AccountService);
  closeMenu = output<void>();

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.closeMenu.emit();
    }
  }

  constructor(
    private router: Router,
    private elementRef: ElementRef,
  ) {}

  onSignOutClick() {
    this.accountService.signOut();
    this.router.navigateByUrl('/');
  }

  onPageLinkClick(href: string) {
    this.closeMenu.emit();

    if (href === '#' || href.trim() === '') return; // temporary guards
    this.router.navigate([href]);
  }

  getProfilePath() {
    return `/users/${this.accountService.currentUser()?.username}`;
  }
}
