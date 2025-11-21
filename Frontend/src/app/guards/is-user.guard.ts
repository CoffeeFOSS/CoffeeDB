import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

export const isUserGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const usernameFromRoute = route.parent?.paramMap.get('username');
  const router = inject(Router);

  if (
    !usernameFromRoute ||
    !accountService.currentUser() ||
    accountService.currentUser()?.username !== usernameFromRoute
  ) {
    router.navigateByUrl('/');
    return false;
  }
  return true;
};
