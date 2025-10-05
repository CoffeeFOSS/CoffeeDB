import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { HotToastService } from '@ngxpert/hot-toast';

export const signedInGuard: CanActivateFn = (route, state) => {
  // Guard to check if user is simply logged in / authenticated.
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toast = inject(HotToastService);

  if (!accountService.currentUser()) {
    router.navigateByUrl('/');
    toast.error('You cannot access this page');
    return false;
  }
  return true;
};
